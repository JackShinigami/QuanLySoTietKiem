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

namespace UI_for_test
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PhieuGuiWindow : Window
    {
        public PhieuGuiWindow()
        {
            InitializeComponent();
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            var cccd = txtKhachHang.Text;
            var maSo = int.Parse(txtMaSo.Text);
            var soTienGui = long.Parse(txtSoTien.Text);
            var ngayGui = DateTime.ParseExact(txtNgayGui.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);


            Business bus = Business.Instance;

            bool result = false;

            try
            {
                result = bus.create_PhieuGui(maSo, cccd, soTienGui, ngayGui);
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
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            int day = DateTime.Now.Day;
            txtNgayGui.Text = day.ToString() + "/" + month.ToString() + "/" + year.ToString();

        }
    }
}
