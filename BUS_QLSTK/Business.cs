
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DAL_QLSTK;
using DTO_QLSTK;
using System.Linq;

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
        public List<dynamic> getList_DoanhSoNgay(DateTime ngay)
        {
            DAL_PhieuGui dal = DAL_PhieuGui.Instance;
            DAL_PhieuRut dal2 = DAL_PhieuRut.Instance;
            DAL_SoTietKiem dal3 = DAL_SoTietKiem.Instance;
            DAL_Config dal4 = DAL_Config.Instance;

            //lay danh sach cac loai tiet kiem dang co trong config (chi lay ki han)
            var list_LoaiTietKiem1 = DAL_Config.Instance.GetList_LoaiTietKiem().Select(x => new { Loaitietkiem=(int) x.Kyhan });
            //lay danh sách các loại tiết kiệm từng được tạo ra trong sổ tiết kiệm
            var list_LoaiTietKiem2 = dal3.GetList_SoTietKiem().Select(x => new { Loaitietkiem= (int)x.Loaitietkiem }).Distinct();
            //lấy danh sách các loại tiết kiệm từ trước tới nay
            var list_LoaiTietKiem=list_LoaiTietKiem1.Union(list_LoaiTietKiem2);



            //lay phieu gui theo ngay join voi so tiet kiem de lay loai tiet kiem
            var list_PhieuGui = dal.GetList_PhieuGui().Where(x => x.Ngaygui == ngay)
                .Join(dal3.GetList_SoTietKiem(), x => x.Maso, y => y.Maso, (x, y) => new { x, y })
                .Select(x => new { x.x.Maphieugui, x.x.Ngaygui, x.x.Sotien, x.y.Loaitietkiem });
            //lay doanh so gui theo ngay group boi loai tiet kiem
            var list_DoanhSoGui = list_PhieuGui.GroupBy(x => x.Loaitietkiem)
                .Select(x => new { Loaitietkiem = x.Key, Tongtiengui = x.Sum(y => y.Sotien) });
            
            var list_DoanhSoGui2 = from loaitietkiem in list_LoaiTietKiem
                                   join doanhso in list_DoanhSoGui on loaitietkiem.Loaitietkiem equals doanhso.Loaitietkiem into temp
                                   from doanhso in temp.DefaultIfEmpty()
                                   select new { Loaitietkiem = loaitietkiem.Loaitietkiem, Tongtiengui = doanhso == null ? 0 : doanhso.Tongtiengui };




            //lay phieu rut theo ngay join voi so tiet kiem de lay loai tiet kiem
            var list_PhieuRut = dal2.GetList_PhieuRut().Where(x => x.Ngayrut == ngay)
                .Join(dal3.GetList_SoTietKiem(), x => x.Maso, y => y.Maso, (x, y) => new { x, y })
                .Select(x => new { x.x.Maphieurut, x.x.Ngayrut, x.x.Sotien, x.y.Loaitietkiem });
            //lay doanh so rut theo ngay group boi loai tiet kiem
            var list_DoanhSoRut = list_PhieuRut.GroupBy(x => x.Loaitietkiem)
                .Select(x => new { Loaitietkiem = x.Key, Tongtienrut = x.Sum(y => y.Sotien) });

            var list_DoanSoRut2 = from loaitietkiem in list_LoaiTietKiem
                                  join doanhso in list_DoanhSoRut on loaitietkiem.Loaitietkiem equals doanhso.Loaitietkiem into temp
                                  from doanhso in temp.DefaultIfEmpty()
                                  select new { Loaitietkiem = loaitietkiem.Loaitietkiem, Tongtienrut = doanhso == null ? 0 : doanhso.Tongtienrut };

            //full outer join 2 list doanh so gui va rut theo loai tiet kiem
            //full outer join = left join + right join 
            //left join
            var leftjoin= from doanhso in list_DoanhSoGui2
                           join doanhso2 in list_DoanSoRut2 on doanhso.Loaitietkiem equals doanhso2.Loaitietkiem into temp
                           from doanhso2 in temp.DefaultIfEmpty()
                           select new { doanhso.Loaitietkiem, doanhso.Tongtiengui, Tongtienrut = doanhso2 == null ? 0 : doanhso2.Tongtienrut };
            //right join
            var rightjoin = from doanhso2 in list_DoanSoRut2
                            join doanhso in list_DoanhSoGui2 on doanhso2.Loaitietkiem equals doanhso.Loaitietkiem into temp
                            from doanhso in temp.DefaultIfEmpty()
                            select new { doanhso2.Loaitietkiem, Tongtiengui = doanhso == null ? 0 : doanhso.Tongtiengui, doanhso2.Tongtienrut };
            //full outer join
            var list_DoanhSoNgay = leftjoin.Union(rightjoin);

            

            //thêm cột chênh lệch 
            var BaoCaoDoanhSoNgay = list_DoanhSoNgay.Select(x => new { x.Loaitietkiem, x.Tongtiengui, x.Tongtienrut, Chenhlech = x.Tongtiengui - x.Tongtienrut }).OrderBy(x=>x.Loaitietkiem);
            var result = BaoCaoDoanhSoNgay.ToList<dynamic>();
            return result;
        }

        public List<dynamic> getList_BaoCaoDongMoSoThang(int thang, int nam)
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
            //outer join= left join + right join
            //left join
            var leftjoin = from sotietkienmo in list_SoTietKiemMo
                           join sotietkiemdong in list_SoTietKiemDong on sotietkienmo.Ngaymoso equals sotietkiemdong.Ngaydongso into temp
                           from sotietkiemdong in temp.DefaultIfEmpty()
                           select new { Ngay=sotietkienmo.Ngaymoso, sotietkienmo.Soluongmo, Soluongdong = sotietkiemdong == null ? 0 : sotietkiemdong.Soluongdong };
            //right join
            var rightjoin = from sotietkiemdong in list_SoTietKiemDong
                            join sotietkienmo in list_SoTietKiemMo on sotietkiemdong.Ngaydongso equals sotietkienmo.Ngaymoso into temp
                            from sotietkienmo in temp.DefaultIfEmpty()
                            select new { Ngay = sotietkiemdong.Ngaydongso, Soluongmo = sotietkienmo == null ? 0 : sotietkienmo.Soluongmo, sotietkiemdong.Soluongdong };
            //full outer join
            var list_BaoCaoDongMoSoThang = leftjoin.Union(rightjoin);
            //them cot chenh lech
            var list_BaoCaoDongMoSoThang2 = list_BaoCaoDongMoSoThang.Select(x => new { x.Ngay, x.Soluongmo, x.Soluongdong, Chenhlech = x.Soluongmo - x.Soluongdong }).OrderBy(x=>x.Ngay);
            var result = list_BaoCaoDongMoSoThang2.ToList<dynamic>();
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
