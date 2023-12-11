
using System.Reflection.Emit;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DAL_QLSTK;
using DTO_QLSTK;

namespace BUS_QLSTK
{
    public class Business
    {
        bool create_SoTietKiem(string maSo, string CCCD, string tenKH, string diaChi, int kyHan, long soTien)
        {
            return true;
        }
        

        bool create_PhieuGui(string maSo, string CCCD, long soTien)
        {
            return true;
        }

        bool create_PhieuRut(string maSo, string CCCD, long soTien)
        {
            return true;
        }

        int getNew_MaSo()
        {
            DAL_SoTietKiem dal = DAL_SoTietKiem.Instance;
            var list_SoTietKiem = dal.GetList_SoTietKiem();
            var max = list_SoTietKiem.Max(x => x.Maso);
            var res = max + 1;
            return res;
        }

        List<dynamic> getList_SoTietKiem()
        {
            return new List<dynamic>();
        }

        //todo: đồng nhất kiểu Date
        List<dynamic> getList_DoanhSoNgay(Date ngay)
        {
            return new List<dynamic>();
        }

        List<dynamic> getList_BaoCaoDongMoSoThang(int thang, int nam)
        {
            return new List<dynamic>();
        }

        //todo: sửa kiểu dữ liệu dynamic thành LoaiTietKiem
        List<dynamic> getList_LoaiTietKiem()
        {
            return new List<dynamic>();
        }

        bool add_LoaiTietKiem(int kyHan, double laiSuat)
        {
            return true;
        }

        //todo sửa kiểu dữ liệu dynamic thành LoaiTietKiem
        bool delete_LoaiTietKiem(dynamic loaiTietKiem)
        {
            return true;
        }

        bool update_NgayGuiToiThieu(int ngay)
        {
            return true;
        }

        bool update_SoTienGuiToiThieu(int ngay)
        {
            return true;
        }

        int get_NgayGuiToiThieu()
        {
            return 1;
        }   

        int get_SoTienGuiToiThieu()
        {
            return 1;
        }

    }

}
