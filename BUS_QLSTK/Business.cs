
using DAL_QLSTK;
using DTO_QLSTK;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DAL_QLSTK;
using DTO_QLSTK;

namespace BUS_QLSTK
{
    public class Business
    {
        private static Business instance;
        DAL_Config Config;
        DAL_KhachHang KhachHang;
        DAL_PhieuGui PhieuGui;
        DAL_PhieuRut PhieuRut;
        DAL_SoTietKiem SoTietKiem;

        public Business()
        {
            Config = DAL_Config.Instance;
            KhachHang = DAL_KhachHang.Instance;
            PhieuGui = DAL_PhieuGui.Instance;
            PhieuRut = DAL_PhieuRut.Instance;
            SoTietKiem = DAL_SoTietKiem.Instance;
        }

        public static Business Instance
        {
            get {            
                if (instance == null)
                {
                    instance = new Business();
                }
                     
                return instance;
                           
            }
        }
        bool create_SoTietKiem( string CCCD, string tenKH, string diaChi, int kyHan, long soTien)
        {
            var soTietKiem = new SoTietKiem();
            soTietKiem.Maso = getNew_MaSo();
            soTietKiem.Cccd = CCCD;
            var currentDateTime = DateTime.Now;
            soTietKiem.Ngaymoso = currentDateTime;
            soTietKiem.Ngaydongso = null;
            soTietKiem.Sodu = soTien;
            soTietKiem.Loaitietkiem = kyHan;
            soTietKiem.Trangthai = 1;
            

            var ds_LoaiTietKiem = Config.GetList_LoaiTietKiem();

            foreach (var loaiTietKiem in ds_LoaiTietKiem)
            {
                if (loaiTietKiem.Kyhan == kyHan)
                {
                    soTietKiem.Laisuat = loaiTietKiem.Laisuat;
                   
                    break;
                }
            }

            soTietKiem.Songayduocrut = Config.Get_SoNgayGuiToiThieu();
            soTietKiem.Tienguitoithieu = Config.Get_SoTienGuiToiThieu();
            
            

            var kh = new KhachHang();
            kh.Cccd = CCCD;
            kh.Hoten = tenKH;
            kh.Diachi = diaChi;

            var result = true;
            if(soTien < soTietKiem.Tienguitoithieu)
            {
                result = false;
                throw new Exception("Số tiền gửi không đủ");
            }
            else
            {
                try
                {
                    SoTietKiem.Add_SoTietKiem(soTietKiem);
                }
                catch (Exception e)
                {
                    result = false;
                    throw e;
                }
            }

            if (result)
            {
                KhachHang.Add_KhachHang(kh);
            }
              

            return result;  
        }
        

        bool create_PhieuGui(int maSo, string CCCD, long soTien)
        {
            var result = true;

            var listSoTietKiem = SoTietKiem.GetList_SoTietKiem();

            foreach (var soTietKiem in listSoTietKiem)
            {
                if (soTietKiem.Maso == maSo)
                {
                    if(soTietKiem.Trangthai == 0)
                    {
                        result = false;
                        throw new Exception("Sổ đã đóng");
                    }

                    else if(soTietKiem.Loaitietkiem != 0)
                    {
                        result = false;
                        throw new Exception("Sổ không phải loại không kỳ hạn");
                    }

                    else if(soTien < soTietKiem.Tienguitoithieu)
                    {
                        result = false;
                        throw new Exception("Số tiền gửi không đủ");
                    }

                    else
                    {
                        soTietKiem.Sodu += soTien;
                        SoTietKiem.Update_SoTietKiem(soTietKiem);
                        
                    }

                    break;
                }
            }

            if (result)
            {
                var phieuGui = new PhieuGui();
                phieuGui.Maso = maSo;
                phieuGui.Ngaygui = DateTime.Now;
                phieuGui.Sotien = soTien;
                phieuGui.Maphieugui = getNew_MaSo();

                PhieuGui.Add_PhieuGui(phieuGui);
            }

            return result;
        }

        bool create_PhieuRut(int maSo, string CCCD, long? soTien)
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
            DAL_SoTietKiem dal = DAL_SoTietKiem.Instance;
            return new List<dynamic>();
        }

        //todo: đồng nhất kiểu Date
        List<dynamic> getList_DoanhSoNgay(DateTime ngay)
        {
            DAL_PhieuGui dal = DAL_PhieuGui.Instance;
            DAL_PhieuRut dal2 = DAL_PhieuRut.Instance;
            DAL_SoTietKiem dal3 = DAL_SoTietKiem.Instance;
            //lay phieu gui theo ngay join voi so tiet kiem de lay loai tiet kiem
            var list_PhieuGui = dal.GetList_PhieuGui().Where(x => x.Ngaygui == ngay)
                .Join(dal3.GetList_SoTietKiem(), x => x.Maso, y => y.Maso, (x, y) => new { x, y })
                .Select(x => new { x.x.Maphieugui, x.x.Ngaygui, x.x.Sotien, x.y.Loaitietkiem });
            //lay doanh so gui theo ngay group boi loai tiet kiem
            var list_DoanhSoGui = list_PhieuGui.GroupBy(x => x.Loaitietkiem)
                .Select(x => new { Loaitietkiem = x.Key, Tongtiengui = x.Sum(y => y.Sotien) });


            //lay phieu rut theo ngay join voi so tiet kiem de lay loai tiet kiem
            var list_PhieuRut = dal2.GetList_PhieuRut().Where(x => x.Ngayrut == ngay)
                .Join(dal3.GetList_SoTietKiem(), x => x.Maso, y => y.Maso, (x, y) => new { x, y })
                .Select(x => new { x.x.Maphieurut, x.x.Ngayrut, x.x.Sotien, x.y.Loaitietkiem });
            //lay doanh so rut theo ngay group boi loai tiet kiem
            var list_DoanhSoRut = list_PhieuRut.GroupBy(x => x.Loaitietkiem)
                .Select(x => new { Loaitietkiem = x.Key, Tongtienrut = x.Sum(y => y.Sotien) });



            //join 2 list doanh so gui va rut theo loai tiet kiem
            var list_DoanhSoNgay = list_DoanhSoGui.Join(list_DoanhSoRut, x => x.Loaitietkiem, y => y.Loaitietkiem, (x, y) => new { x, y })
                .Select(x => new { x.x.Loaitietkiem, x.x.Tongtiengui, x.y.Tongtienrut, chenhlech= x.x.Tongtiengui - x.y.Tongtienrut });

            var result = list_DoanhSoNgay.ToList<dynamic>();
            return result;
        }

        List<dynamic> getList_BaoCaoDongMoSoThang(int thang, int nam)
        {
            DAL_SoTietKiem dal = DAL_SoTietKiem.Instance;
            //lay cac so tiet kiem mo trong thang group by ngay mo
            var list_SoTietKiemMo = dal.GetList_SoTietKiem().Where(x => x.Ngaymoso.Value.Month == thang && x.Ngaymoso.Value.Year == nam)
                .GroupBy(x => x.Ngaymoso.Value.Date)
                .Select(x => new { Ngaymoso = x.Key, Soluongmo = x.Count() });

            //lay cac so tiet kiem dong trong thang group by ngay dong
            var list_SoTietKiemDong = dal.GetList_SoTietKiem().Where(x => x.Ngaydongso.Value.Month == thang && x.Ngaydongso.Value.Year == nam)
                .GroupBy(x => x.Ngaydongso.Value.Date)
                .Select(x => new { Ngaydongso = x.Key, Soluongdong = x.Count() });
            //join 2 list so tiet kiem mo va dong theo ngay va tinh chenh lech
            var list_BaoCaoDongMoSoThang = list_SoTietKiemMo.Join(list_SoTietKiemDong, x => x.Ngaymoso, y => y.Ngaydongso, (x, y) => new { x, y })
                .Select(x => new { x.x.Ngaymoso, x.x.Soluongmo, x.y.Soluongdong, Chenhlech = x.x.Soluongmo - x.y.Soluongdong });

            var result = list_BaoCaoDongMoSoThang.ToList<dynamic>();
            return result;
        }

        List<LoaiTietKiem> getList_LoaiTietKiem()
        {
            return new List<LoaiTietKiem>();
        }

        bool add_LoaiTietKiem(int kyHan, double laiSuat)
        {
            return true;
        }

       
        bool delete_LoaiTietKiem(LoaiTietKiem loaiTietKiem)
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
