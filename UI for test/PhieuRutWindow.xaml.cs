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
    public partial class PhieuRutWindow : Window
    {
        public PhieuRutWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var day = DateTime.Now.Day;
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            txtNgaRut.Text = $"{day}/{month}/{year}";
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            var maSo = int.Parse(txtMaSo.Text);
            var soTienRut = long.Parse(txtSoTien.Text);
            var cccd = txtKhachHang.Text;
            var ngayRut  = DateTime.ParseExact(txtNgaRut.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            Business bus = Business.Instance;

            bool result = false;
            try
            {
                result = bus.create_PhieuRut(maSo, cccd, soTienRut, ngayRut);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (result)
            {
                MessageBox.Show("Tạo phiếu rút thành công");
                this.Close();
            }

        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            long? soTienDuocRut = 0;

            var maSo = int.Parse(txtMaSo.Text);
            var cccd = txtKhachHang.Text;
            try
            {
                soTienDuocRut = Business.Instance.get_SoTienCoTheRut(maSo, cccd);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if(soTienDuocRut != 0)
            {
                if(soTienDuocRut > 0)
                {
                    txtSoTien.Text = soTienDuocRut.ToString();
                    txtSoTien.IsReadOnly = true;
                }
                else
                {
                    soTienDuocRut = -soTienDuocRut;
                    txtSoTien.Text = soTienDuocRut.ToString();
                }
            }
        }
    }
}
