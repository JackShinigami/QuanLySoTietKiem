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

namespace UI_for_test
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

        

        private void btnSTK_Click(object sender, RoutedEventArgs e)
        {
            var stkWindow = new STKWindow();
            stkWindow.ShowDialog();

           
        }

        private void btnPhieuGui_Click(object sender, RoutedEventArgs e)
        {
            var phieuGuiWindow = new PhieuGuiWindow();
            phieuGuiWindow.ShowDialog();

        }

        private void btnPhieuRut_Click(object sender, RoutedEventArgs e)
        {
            var phieuRutWindow = new PhieuRutWindow();
            phieuRutWindow.ShowDialog();
        }
    }
}