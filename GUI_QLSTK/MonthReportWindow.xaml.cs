using BUS_QLSTK;
using DTO_QLSTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI_QLSTK
{
    /// <summary>
    /// Interaction logic for MonthReportWindow.xaml
    /// </summary>
    public partial class MonthReportWindow : Window
    {

        public string ReportDate { get; set; }

        public bool IsLoading { get; set; }
        Business business = Business.Instance;

        public MonthReportWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoading = true;
            progressBar.Visibility = Visibility.Visible;
            try
            {
                var list = new List<LoaiTietKiem>();
                await Task.Run(() =>
                {
                    list = business.getList_LoaiTietKiemInHistory();
                    IsLoading = false;
                });

                CultureInfo cultureInfo = new CultureInfo("vi-VN");
                cultureInfo.DateTimeFormat.ShortDatePattern = "MM/yyyy";
                cultureInfo.DateTimeFormat.DateSeparator = "/";
                Thread.CurrentThread.CurrentCulture = cultureInfo;

                reportDatePicker.SelectedDate = DateTime.Now;
                ReportDate = reportDatePicker.SelectedDate!.Value.ToShortDateString();
                // thay đổi datepicker sang chế độ chọn tháng
                reportDatePicker.DisplayDateEnd = DateTime.Now;
                while (IsLoading)
                {
                    // wait for loading to finish
                }
                periodTypeComboBox.ItemsSource = list;
            }
            catch (Exception ex)
            {
                errorTextBlock.Text = ex.Message;
            }
            progressBar.Visibility = Visibility.Hidden;
        }

        private async void reportButton_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            errorTextBlock.Text = "";
            IsLoading = true;
            try
            {

                var list = new List<dynamic>();
                DateTime date = DateTime.Now;
                try
                {
                    date = reportDatePicker.SelectedDate!.Value;
                }
                catch (Exception)
                {
                    throw new Exception("Vui lòng chọn tháng báo cáo");
                }
                var month = (int)date.Month;
                var year = (int)date.Year;
                var periodType = periodTypeComboBox.SelectedItem as LoaiTietKiem;
                int period = 0;
                if (periodType != null)
                {
                    period = periodType.Kyhan;
                } else
                {
                    throw new Exception("Vui lòng chọn loại tiết kiệm");
                }

                await Task.Run(() =>
                {
                    list = business.getList_BaoCaoDongMoSoThang(month, year, period);
                    IsLoading = false;
                });

                while (IsLoading)
                {
                    // wait for loading to finish
                }
                reportDataGrid.ItemsSource = list;
                if (list.Count == 0)
                {
                    throw new Exception("Không có hoạt động đóng mở/sổ nào");
                }
            }
            catch (Exception ex)
            {
                errorTextBlock.Text = ex.Message;
            }
            progressBar.Visibility = Visibility.Hidden;

        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void reportDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CultureInfo cultureInfo = new CultureInfo("vi-VN");
            cultureInfo.DateTimeFormat.ShortDatePattern = "MM/yyyy";
            cultureInfo.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            // Xoá ngày khỏi ReportDate (sang dạng MM/yyyy)
        }

        private void reportDatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            // move to selected month mode

            CultureInfo cultureInfo = new CultureInfo("vi-VN");
            cultureInfo.DateTimeFormat.ShortDatePattern = "MM/yyyy";
            cultureInfo.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            var datePicker = sender as DatePicker;
            var popup = datePicker?.Template.FindName("PART_Popup", datePicker) as Popup;
            var calendar = popup?.Child as System.Windows.Controls.Calendar;
            if (calendar != null)
            {
                calendar.DisplayMode = CalendarMode.Year;

                calendar.DisplayModeChanged += (s, ev) =>
                {
                    if (calendar.DisplayMode == CalendarMode.Month)
                    {
                        // chọn đại ngày 1 của tháng (tại khom biết tắt hộp thoại khi mới đổi tháng)
                        calendar.SelectedDate = new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, 1);
                        reportDatePicker.SelectedDate = calendar.SelectedDate;
                        reportDatePicker.IsDropDownOpen = false;
                    } else
                    {
                        reportDatePicker.IsDropDownOpen = true;
                    }
                };

            }

        }


    }
}
