using BUS_QLSTK;
using DTO_QLSTK;
using System;
using System.Collections.Generic;
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
    public partial class STKWindow : Window
    {

        public STKWindow()
        {
            InitializeComponent();
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            Business bus = Business.Instance;

            bool result = false;

            var loaiTietKiem = (LoaiTietKiem) cb_LoaiTietKiem.SelectedItem ;

            var kyHan = loaiTietKiem.Kyhan;

            long soTien = long.Parse(txtSoTien.Text) ;

            var maSo = int.Parse(txtMaSo.Text);
            var cccd = txtCCCD.Text;
            var tenKH = txtKhachHang.Text;
            var diaChi = txtDiaChi.Text;

            try
            {
                result = bus.create_SoTietKiem(maSo, cccd, tenKH, diaChi, kyHan,  soTien);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (result)
            {
                MessageBox.Show("Tạo sổ tiết kiệm thành công");
                this.Close();
            }
            else
            {
                MessageBox.Show("Tạo sổ tiết kiệm thất bại");
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtMaSo.Text = Business.Instance.getNew_MaSo().ToString();
            txtMaSo.IsReadOnly = true;

            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            txtNgayMoSo.Text = $"{day}/{month}/{year}";
            txtNgayMoSo.IsReadOnly = true;

            cb_LoaiTietKiem.ItemsSource = Business.Instance.getList_LoaiTietKiem();
            cb_LoaiTietKiem.SelectedIndex = 0;
        }
    }
}
