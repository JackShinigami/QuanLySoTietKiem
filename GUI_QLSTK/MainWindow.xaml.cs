using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI_QLSTK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void createBookButton_Click(object sender, RoutedEventArgs e)
        {
            CreateBookWindow createBookWindow = new CreateBookWindow();
            createBookWindow.ShowDialog();
        }

        private void createDepositButton_Click(object sender, RoutedEventArgs e)
        {
            DepositFormWindow depositFormWindow = new DepositFormWindow();
            depositFormWindow.ShowDialog();
        }

        private void createWithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            WithdrawFormWindow withdrawFormWindow = new WithdrawFormWindow();
            withdrawFormWindow.ShowDialog();

        }

        private void lookUpBookButton_Click(object sender, RoutedEventArgs e)
        {
            LookUpWindow lookUpWindow = new LookUpWindow();
            lookUpWindow.ShowDialog();
        }


        private void reportMonthButton_Click(object sender, RoutedEventArgs e)
        {
            MonthReportWindow monthReportWindow = new MonthReportWindow();
            monthReportWindow.ShowDialog();
        }

        private void reportDayButton_Click(object sender, RoutedEventArgs e)
        {
            DayReportWindow dayReportWindow = new DayReportWindow();
            dayReportWindow.ShowDialog();
        }


        private void manageRegulationButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Điều hướng sang màn hình quản lý quy định
        }


    }
}