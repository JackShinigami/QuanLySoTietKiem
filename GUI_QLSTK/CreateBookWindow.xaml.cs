using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using BUS_QLSTK;
using DTO_QLSTK;

namespace GUI_QLSTK
{
    /// <summary>
    /// Interaction logic for CreateBook.xaml
    /// </summary>


    public partial class CreateBookWindow : Window, INotifyPropertyChanged
    {

        Business bus = Business.Instance;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string OpenDate { get; set; }



        public CreateBookWindow()
        {
            InitializeComponent();
            //TODO: tải từ cơ sở dữ liệu thay vì gán cứng


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CultureInfo cultureInfo = new CultureInfo("vi-VN");
                cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                cultureInfo.DateTimeFormat.DateSeparator = "/";
                Thread.CurrentThread.CurrentCulture = cultureInfo;

                openDateDatePicker.SelectedDate = DateTime.Now;
                OpenDate = openDateDatePicker.SelectedDate!.Value.ToShortDateString();

                bookIdTextBox.Text = bus.getNew_MaSo().ToString();
                bookIdTextBox.IsReadOnly = true;

                periodTypeComboBox.ItemsSource = bus.getList_LoaiTietKiem();
                this.DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void manageRegulationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void createBookButton_Click(object sender, RoutedEventArgs e)
        {


            bool result = false;

            try
            {
                if(customerTextBox.Text == "")
                {
                    throw new Exception("Vui lòng nhập tên khách hàng");
                }
                if(idCardTextBox.Text == "")
                {
                    throw new Exception("Vui lòng nhập số CMND");
                }
                if(addressTextBox.Text == "")
                {
                    throw new Exception("Vui lòng nhập địa chỉ");
                }
                if(depositTextBox.Text == "")
                {
                    throw new Exception("Vui lòng nhập số tiền gửi");
                }
                if(periodTypeComboBox.SelectedItem == null)
                {
                    throw new Exception("Vui lòng chọn loại tiết kiệm");
                }

                int id = int.Parse(bookIdTextBox.Text);
                var periodType = periodTypeComboBox.SelectedItem as LoaiTietKiem;
                int period = periodType!.Kyhan;
                long deposit = long.Parse(depositTextBox.Text);
                string idCard = idCardTextBox.Text;
                string customer = customerTextBox.Text;
                string address = addressTextBox.Text;

                if(openDateDatePicker.SelectedDate == null)
                {
                    throw new Exception("Vui lòng chọn ngày mở sổ");
                }

                DateTime openDate = openDateDatePicker.SelectedDate!.Value;

                result = bus.create_SoTietKiem(id, idCard, customer, address, period, deposit, openDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (result)
            {
                MessageBox.Show("Tạo sổ tiết kiệm thành công");
                this.Close();
            } else
            {
                MessageBox.Show("Tạo sổ tiết kiệm thất bại");
            }

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
