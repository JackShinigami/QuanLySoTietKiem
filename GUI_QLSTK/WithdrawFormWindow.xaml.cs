using BUS_QLSTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI_QLSTK
{
    /// <summary>
    /// Interaction logic for WithdrawFormWindow.xaml
    /// </summary>
    public partial class WithdrawFormWindow : Window
    {

        Business bus = Business.Instance;
        public string WithdrawDate;

        public bool IsLoading { get; set; }

        public WithdrawFormWindow()
        {
            InitializeComponent();
        }

        private async void completeFormButton_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;

            try
            {
                if (withdrawTextBox.Text == "")
                {
                    throw new Exception("Vui lòng nhập số tiền rút");
                }

                var withdraw = long.Parse(withdrawTextBox.Text);
                var BookId = int.Parse(bookIdTextBox.Text);
                var CustomerID = customerIDTextBox.Text;

                if (withdrawDateDatePicker.SelectedDate == null)
                {
                    throw new Exception("Vui lòng nhập ngày rút");
                }
                if (withdraw <= 0)
                {
                    throw new Exception("Số tiền rút phải lớn hơn 0");
                }

                var withdrawDate = withdrawDateDatePicker.SelectedDate!.Value;
                await Task.Run(() =>
                {
                    result = bus.create_PhieuRut(BookId, CustomerID, withdraw, withdrawDate);
                    IsLoading = false;
                }); 
                while (IsLoading)
                {
                    // wait for IsLoading to be false
                }
            }
            catch (Exception ex)
            {
                infoFaultStackPanel.Visibility = Visibility.Visible;
                infoStackPanel.Visibility = Visibility.Collapsed;
                infoFaultLabel.Content = ex.Message;
            }

            if (result)
            {
                MessageBox.Show("Tạo phiếu rút thành công");
                this.Close();
            }
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CultureInfo cultureInfo = new CultureInfo("vi-VN");
            cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cultureInfo.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            withdrawDateDatePicker.SelectedDate = DateTime.Now;
            WithdrawDate = withdrawDateDatePicker.SelectedDate!.Value.ToShortDateString();
            withdrawDateDatePicker.Text = WithdrawDate;

        }

        private async void checkInfoButton_Click(object sender, RoutedEventArgs e)
        {
            long? result = null;
            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;

            try
            {

                if (bookIdTextBox.Text == "")
                {
                    throw new Exception("Vui lòng nhập mã sổ");
                }
                if (customerIDTextBox.Text == "")
                {
                    throw new Exception("Vui lòng nhập mã khách hàng");
                }

                var bookId = int.Parse(bookIdTextBox.Text);
                var customerId = customerIDTextBox.Text;

                if (withdrawDateDatePicker.SelectedDate == null)
                {
                    throw new Exception("Vui lòng chọn ngày mở sổ");
                }

                DateTime openDate = withdrawDateDatePicker.SelectedDate!.Value;

                try
                {

                    await Task.Run(() =>
                    {
                        result = bus.get_SoTienCoTheRut(bookId, customerId,openDate);
                        IsLoading = false;
                    });
                    while (IsLoading)
                    {
                        // wait for IsLoading to be false
                    }

                    if (result != null)
                    {

                        if (result < 0) // Số tiền sổ không kì hạn
                        {
                            infoStackPanel.Visibility = Visibility.Visible;
                            infoFaultStackPanel.Visibility = Visibility.Collapsed;
                            bookIdLabel.Content = bookIdTextBox.Text;
                            customerIdLabel.Content = customerIDTextBox.Text;
                            withdrawableAmountLabel.Content = -result;
                            withdrawTextBox.IsEnabled = true;
                            withdrawTextBox.Text = (-result).ToString();
                            completeFormButton.IsEnabled = true;

                        } 
                        else // Số tiền sổ kì hạn
                        {
                            infoStackPanel.Visibility = Visibility.Visible;
                            infoFaultStackPanel.Visibility = Visibility.Collapsed;
                            bookIdLabel.Content = bookIdTextBox.Text;
                            customerIdLabel.Content = customerIDTextBox.Text;
                            withdrawableAmountLabel.Content = result;
                            withdrawTextBox.IsEnabled = false;
                            withdrawTextBox.Text = result.ToString();
                            completeFormButton.IsEnabled = true;
                        }
                    }
                    else
                    {
                        // không có null nên không làm gì
                    }

                } catch (Exception ex)
                {
                    infoFaultStackPanel.Visibility = Visibility.Visible;
                    infoStackPanel.Visibility = Visibility.Collapsed;
                    completeFormButton.IsEnabled = false;
                    withdrawTextBox.Text = "";
                    infoFaultLabel.Content = ex.Message;
                }
            }
            catch (Exception ex)
            {
                infoFaultStackPanel.Visibility = Visibility.Visible;
                infoStackPanel.Visibility = Visibility.Collapsed;
                completeFormButton.IsEnabled = false;
                withdrawTextBox.Text = "";
                infoFaultLabel.Content = ex.Message;
            }
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void manageRegulationButton_Click(object sender, RoutedEventArgs e)
        {
            ManageConfigWindow window = new ManageConfigWindow();
            window.ShowDialog();
        }
    }
}
