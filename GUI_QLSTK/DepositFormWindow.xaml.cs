using BUS_QLSTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for DepositFormWindow.xaml
    /// </summary>
    public partial class DepositFormWindow : Window, INotifyPropertyChanged
    {

        public string DepositDate { get; set; }

        Business bus = Business.Instance;

        public DepositFormWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void completeFormButton_Click(object sender, RoutedEventArgs e)
        {


            bool result = false;

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
                if (depositTextBox.Text == "")
                {
                    throw new Exception("Vui lòng nhập số tiền gửi");
                }

                var deposit = long.Parse(depositTextBox.Text);
                var BookId = int.Parse(bookIdTextBox.Text);
                var CustomerID = customerIDTextBox.Text;

                if (depositDateDatePicker.SelectedDate == null)
                {
                    throw new Exception("Vui lòng nhập ngày gửi");
                }
                if(deposit <= 0)
                {
                    throw new Exception("Số tiền gửi phải lớn hơn 0");
                }

                var depositDate = depositDateDatePicker.SelectedDate!.Value;

                result = bus.create_PhieuGui(BookId, CustomerID, deposit, depositDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (result)
            {
                MessageBox.Show("Tạo phiếu gửi thành công");
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CultureInfo cultureInfo = new CultureInfo("vi-VN");
                cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                cultureInfo.DateTimeFormat.DateSeparator = "/";
                Thread.CurrentThread.CurrentCulture = cultureInfo;

                depositDateDatePicker.SelectedDate = DateTime.Now;
                DepositDate = depositDateDatePicker.SelectedDate!.Value.ToShortDateString();
                depositDateDatePicker.Text = DepositDate;

                /*
                openDateDatePicker.SelectedDate = DateTime.Now;
                OpenDate = openDateDatePicker.SelectedDate!.Value.ToShortDateString();
                 */

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
