using BUS_QLSTK;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace GUI_QLSTK
{
    /// <summary>
    /// Interaction logic for LookUpWindow.xaml
    /// </summary>
    public partial class LookUpWindow : Window
    {

        Business bus = Business.Instance;

        public LookUpWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            progressBar.Visibility = Visibility.Visible;

            bool loading = true;
            var list = new List<dynamic>();
            await Task.Run(() =>
            {
                list = bus.getList_SoTietKiem();
                loading = false;
            });

            while (loading) 
            {
                // wait for loading to finish
            }
            lookUpDataGrid.ItemsSource = list;

 
            progressBar.Visibility = Visibility.Hidden;

        }


        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
