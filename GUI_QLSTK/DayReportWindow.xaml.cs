using BUS_QLSTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
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
    /// Interaction logic for DayReportWindow.xaml
    /// </summary>
    public partial class DayReportWindow : Window, INotifyPropertyChanged
    {

        public string ReportDate { get; set; }

        public bool IsLoading { get; set; }
        Business business = Business.Instance;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DayReportWindow()
        {

            CultureInfo cultureInfo = new CultureInfo("vi-VN");
            cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cultureInfo.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            ReportDate = DateTime.Now.ToShortDateString();
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            reportDatePicker.SelectedDate = DateTime.Now;
            ReportDate = reportDatePicker.SelectedDate!.Value.ToShortDateString();
            reportDatePicker.Text = ReportDate;

        }

        private async Task reportButton_ClickAsync(object sender, RoutedEventArgs e, DateTime date)
        {

            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;
            try
            {
                var list = new List<dynamic>();
                await Task.Run(() =>
                {
                    list = business.getList_DoanhSoNgay(date);
                    IsLoading = false;
                });

                while (IsLoading)
                {
                    // wait for loading to finish
                }
                reportDataGrid.ItemsSource = list;
            } catch (Exception ex)
            {
                errorTextBlock.Text = ex.Message;
            }
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void reportButton_Click(object sender, RoutedEventArgs e)
        {

            DateTime dateTime = reportDatePicker.SelectedDate!.Value;
            _ = reportButton_ClickAsync(sender, e, dateTime);
        }
    }
}
