using BUS_QLSTK;
using System;
using System.Collections.Generic;
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
    public partial class DayReportWindow : Window
    {

        public string ReportDate { get; set; }

        public bool IsLoading { get; set; }
        Business business = Business.Instance;

        public DayReportWindow()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CultureInfo cultureInfo = new CultureInfo("vi-VN");
            cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cultureInfo.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            reportDatePicker.SelectedDate = DateTime.Now;
            ReportDate = reportDatePicker.SelectedDate!.Value.ToShortDateString();

        }

        private async Task reportButton_ClickAsync(object sender, RoutedEventArgs e)
        {

            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;
            var list = new List<dynamic>();
            await Task.Run(() =>
            {
                DateTime dateTime = DateTime.ParseExact(ReportDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                list = business.getList_DoanhSoNgay(dateTime);
                IsLoading = false;
            });

            while (IsLoading)
            {
                // wait for loading to finish
            }
            reportDataGrid.ItemsSource = list;
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void reportButton_Click(object sender, RoutedEventArgs e)
        {
            _ = reportButton_ClickAsync(sender, e);
        }
    }
}
