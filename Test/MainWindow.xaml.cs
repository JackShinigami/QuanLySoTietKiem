using BUS_QLSTK;
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

namespace Test
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Business bus = Business.Instance;
            DateTime ngay = DateTime.Parse("2023-03-12");
            //var baocaodoanhso = bus.getList_DoanhSoNgay(ngay);
            //MessageBox.Show(baocaodoanhso.Count.ToString());

            var baocaodongmosothang = bus.getList_BaoCaoDongMoSoThang(1,2023);

        }
    }
}