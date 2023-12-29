using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DAL_QLSTK;
using DTO_QLSTK;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;

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
        public bool create_SoTietKiem(int maSo, string CCCD, string tenKH, string diaChi, int kyHan, long soTien, DateTime ngayMoSo)
        {
            var soTietKiem = new SoTietKiem();
            soTietKiem.Maso = maSo;
            soTietKiem.Cccd = CCCD;
            soTietKiem.Ngaymoso = ngayMoSo;
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
                    try
                    {
                        KhachHang.Add_KhachHang(kh);
                    }
                    catch (Exception e) {
                       
                    }
                    SoTietKiem.Add_SoTietKiem(soTietKiem);
                }
                catch (Exception e)
                {
                    result = false;
                    throw new Exception(e.Message);
                }
            }

            if (result)
            {
                var phieuGui = new PhieuGui();
                phieuGui.Maso = soTietKiem.Maso;
                phieuGui.Ngaygui = ngayMoSo;
                phieuGui.Sotien = soTien;
                phieuGui.Maphieugui = getNew_MaPhieuGui();
                PhieuGui.Add_PhieuGui(phieuGui);
                
            }
              

            return result;  
        }

        public long? get_SoTienCoTheRut(int maSo, string CCCD)
        {
            long? result = 0;

            var listSoTietKiem = SoTietKiem.GetList_SoTietKiem();

            SoTietKiem? soTietKiem = null;

            foreach (var soTK in listSoTietKiem)
            {
                if (soTK.Maso == maSo)
                {

                    soTietKiem = soTK;

                    break;
                }
            }

            if (soTietKiem == null)
            {
                throw new Exception("Không tìm thấy sổ tiết kiệm");
            }

            if (soTietKiem.Trangthai == 0)
            {

                throw new Exception("Sổ đã đóng");
            }
            else if(soTietKiem.Cccd.Trim() != CCCD.Trim())
            {
                throw new Exception("Số CCCD không đúng");
            }

            else if (soTietKiem.Loaitietkiem == 0)
            {
                var soNgayGui = (DateTime.Now - soTietKiem.Ngaymoso)?.Days;
                var listPhieuRut = PhieuRut.GetList_PhieuRut();


                if(soNgayGui < soTietKiem.Songayduocrut)
                {
                    throw new Exception("Sổ chưa đủ ngày gửi");
                }

                DateTime? latestDate = DateTime.MinValue;

                foreach (var phieuRut in listPhieuRut)
                {
                    if (phieuRut.Maso == maSo)
                    {
                        latestDate = phieuRut?.Ngayrut > latestDate ? phieuRut.Ngayrut : latestDate;
                    }
                }

                var soThangGui = (DateTime.Now - soTietKiem.Ngaymoso)?.Days / 30;

                if (latestDate != DateTime.MinValue)
                {
                    var soThangDaTinhLai = (latestDate - soTietKiem.Ngaymoso)?.Days / 30;

                    soThangGui -= soThangDaTinhLai;
                }

                result = -(soTietKiem.Sodu + (long)(soTietKiem.Sodu * soTietKiem.Laisuat * soThangGui));

            }

            else
            {
                var soNgayGui = (DateTime.Now - soTietKiem.Ngaymoso)?.Days;
                var soThangGui = soNgayGui / 30;

                if (soThangGui < soTietKiem.Loaitietkiem)
                {
                    throw new Exception("Sổ chưa đủ kỳ hạn");
                }
                else
                {
                    var soLanDaoHan = soThangGui / soTietKiem.Loaitietkiem;
                    result = (long)(soTietKiem.Sodu * soTietKiem.Laisuat * soLanDaoHan * soTietKiem.Loaitietkiem + soTietKiem.Sodu);

                }
            }

            return result;
        }


        public bool create_PhieuGui(int maSo, string CCCD, long soTien, DateTime ngayGui)
        {
            var result = true;

            var listSoTietKiem = SoTietKiem.GetList_SoTietKiem();
            SoTietKiem? soTK = null;

            foreach (var soTietKiem in listSoTietKiem)
            {
                if (soTietKiem.Maso == maSo)
                {
                    soTK = soTietKiem;

                    break;
                }
            }
            if(soTK == null)
            {
                result =false;
                throw new Exception("Không tìm thấy sổ tiết kiệm");
            }

            if (soTK.Trangthai == 0)
            {
                result = false;
                throw new Exception("Sổ đã đóng");
            }

            else if (soTK.Loaitietkiem != 0)
            {
                result = false;
                throw new Exception("Sổ không phải loại không kỳ hạn");
            }

            else if (soTien < soTK.Tienguitoithieu)
            {
                result = false;
                throw new Exception("Số tiền gửi không đủ");
            }
            else if (CCCD.Trim() != soTK.Cccd.Trim())
            {
                result = false;
                throw new Exception("Số CCCD không đúng");
            }

            else
            {
                soTK.Sodu += soTien;
                SoTietKiem.Update_SoTietKiem(soTK);

            }

            if (result)
            {
                var phieuGui = new PhieuGui();
                phieuGui.Maso = maSo;
                phieuGui.Ngaygui = ngayGui;
                phieuGui.Sotien = soTien;
                phieuGui.Maphieugui = getNew_MaPhieuGui();

                PhieuGui.Add_PhieuGui(phieuGui);
            }

            return result;
        }

        public bool create_PhieuRut(int maSo, string CCCD, long? soTien, DateTime ngayRut)
        {
            var result = true;

            var listSoTietKiem = SoTietKiem.GetList_SoTietKiem();

            SoTietKiem? soTietKiem = null;

            foreach(var soTK in listSoTietKiem)
            {
                if(soTK.Maso == maSo)
                {

                    soTietKiem = soTK;
                    
                    break;
                }
            }

            if(soTietKiem == null)
            {
                result = false;
                throw new Exception("Không tìm thấy sổ tiết kiệm");
            }

            if (soTietKiem.Trangthai == 0)
            {
                result = false;
                throw new Exception("Sổ đã đóng");
            }
            else if(soTietKiem.Cccd.Trim() != CCCD.Trim())
            {
                result = false;
                throw new Exception("Số CCCD không đúng");
            }

            else if (soTietKiem.Loaitietkiem == 0)
            {
                var soNgayGui = (DateTime.Now - soTietKiem.Ngaymoso)?.Days;

                if(soNgayGui < soTietKiem.Songayduocrut)
                {
                    result = false;
                    throw new Exception("Sổ chưa đủ ngày gửi");
                }
                else
                {
                    var listPhieuRut = PhieuRut.GetList_PhieuRut();

                    DateTime? latestDate = DateTime.MinValue;

                    foreach (var phieuRut in listPhieuRut)
                    {
                        if (phieuRut.Maso == maSo)
                        {
                            latestDate = phieuRut?.Ngayrut > latestDate ? phieuRut.Ngayrut : latestDate;
                        }
                    }

                    var soThangGui = (DateTime.Now - soTietKiem.Ngaymoso)?.Days / 30;

                    if (latestDate != DateTime.MinValue)
                    {
                        var soThangDaTinhLai = (latestDate - soTietKiem.Ngaymoso)?.Days / 30;
                        
                        soThangGui -= soThangDaTinhLai;
                    }

                    soTietKiem.Sodu += (long)(soTietKiem.Sodu * soTietKiem.Laisuat * soThangGui );

                    if(soTien > soTietKiem.Sodu)
                    {
                        result = false;
                        throw new Exception("Số tiền rút lớn hơn số dư");
                    }
                    else
                    {
                        soTietKiem.Sodu -= soTien;
                        if(soTietKiem.Sodu == 0)
                        {
                            soTietKiem.Trangthai = 0;
                            soTietKiem.Ngaydongso = ngayRut;
                        }
                        SoTietKiem.Update_SoTietKiem(soTietKiem);

                        var phieuRut = new PhieuRut();
                        phieuRut.Maso = maSo;
                        phieuRut.Ngayrut = ngayRut;
                        phieuRut.Sotien = soTien;
                        phieuRut.Maphieurut = getNew_MaPhieuRut();
                        PhieuRut.Add_PhieuRut(phieuRut);

                    }
                }
            }

            else
            {
                var soNgayGui = (DateTime.Now - soTietKiem.Ngaymoso)?.Days;
                var soThangGui = soNgayGui / 30;

                if(soThangGui < soTietKiem.Loaitietkiem)
                {
                    result = false;
                    throw new Exception("Sổ chưa đủ kỳ hạn");
                }
                else
                {
                    var soLanDaoHan = soThangGui / soTietKiem.Loaitietkiem;
                   soTietKiem.Sodu += (long)(soTietKiem.Sodu * soTietKiem.Laisuat * soLanDaoHan * soTietKiem.Loaitietkiem);
                    soTien = soTietKiem.Sodu;
                    soTietKiem.Sodu = 0;
                    soTietKiem.Trangthai = 0;
                    soTietKiem.Ngaydongso = ngayRut;
                    SoTietKiem.Update_SoTietKiem(soTietKiem);

                    var phieuRut = new PhieuRut();
                    phieuRut.Maso = maSo;
                    phieuRut.Ngayrut = ngayRut;
                    phieuRut.Sotien = soTien;
                    phieuRut.Maphieurut = getNew_MaPhieuRut();
                    PhieuRut.Add_PhieuRut(phieuRut);
                    
                }
            }

            return result;

            
        }

        public int getNew_MaSo()
        {
            DAL_SoTietKiem dal = DAL_SoTietKiem.Instance;
            var list_SoTietKiem = dal.GetList_SoTietKiem();

            if(list_SoTietKiem.Count == 0)
            {
                return 1000001;
            }
            var max = list_SoTietKiem.Max(x => x.Maso);
            var res = max + 1;
            return res;
        }

        public int getNew_MaPhieuGui()
        {
            var list_PhieuGui = PhieuGui.GetList_PhieuGui();
            if (list_PhieuGui.Count == 0)
            {
                return 1000001;
            }
            var max = list_PhieuGui.Max(x => x.Maphieugui);
            var res = max + 1;
            return res;
        }


        public int getNew_MaPhieuRut()
        {
            var list_PhieuRut = PhieuRut.GetList_PhieuRut();
            if (list_PhieuRut.Count == 0)
            {
                return 1000001;
            }
            var max = list_PhieuRut.Max(x => x.Maphieurut);
            var res = max + 1;
            return res;
        }

        public List<dynamic> getList_SoTietKiem()
        {
            DAL_SoTietKiem soTietKiem = DAL_SoTietKiem.Instance;
            DAL_KhachHang khachHang = DAL_KhachHang.Instance;

            var list_SoTietKiem = soTietKiem.GetList_SoTietKiem();  
            var list_KhachHang = khachHang.GetList_KhachHang();
            var list_Stt = Enumerable.Range(1, list_SoTietKiem.Count).ToList();

            var list_SoTietKiemJoinKhachHang = list_SoTietKiem.Join(list_KhachHang, x => x.Cccd, y => y.Cccd, (x, y) => new { x.Maso, x.Loaitietkiem, x.Cccd, y.Hoten, x.Sodu });
            var result = list_SoTietKiemJoinKhachHang.Select((x, index) => new { Stt = list_Stt[index], x.Maso, x.Loaitietkiem, x.Cccd, x.Hoten, x.Sodu });

            return result.ToList<dynamic>();

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
            var list_PhieuGui = dal.GetList_PhieuGui().Where(x => x.Ngaygui.Value.Date==ngay.Date && x.Ngaygui.Value.Month==ngay.Month && x.Ngaygui.Value.Year==ngay.Year)
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
            var list_PhieuRut = dal2.GetList_PhieuRut().Where(x => x.Ngayrut.Value.Date == ngay.Date && x.Ngayrut.Value.Month == ngay.Month && x.Ngayrut.Value.Year==ngay.Year)
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
            var list_Stt = Enumerable.Range(1, BaoCaoDoanhSoNgay.Count()).ToList();
            var BaoCaoDoanhSoNgay2 = BaoCaoDoanhSoNgay.Select((x, index) => new { Stt = list_Stt[index], x.Loaitietkiem, x.Tongtiengui, x.Tongtienrut, x.Chenhlech });
            var result = BaoCaoDoanhSoNgay2.ToList<dynamic>();
            return result;
        }

        public List<dynamic> getList_BaoCaoDongMoSoThang(int thang, int nam, int loaitk)
        {
            DAL_SoTietKiem dal = DAL_SoTietKiem.Instance;
            //lay cac so tiet kiem mo trong thang group by ngay mo
            var list_SoTietKiemMo = dal.GetList_SoTietKiem().Where(x => x.Ngaymoso!.Value.Month == thang && x.Ngaymoso!.Value.Year == nam && x.Loaitietkiem==loaitk)
                .GroupBy(x => x.Ngaymoso!.Value.Date)
                .Select(x => new { Ngaymoso = x.Key, Soluongmo = x.Count() });

            //lay cac so tiet kiem dong trong thang group by ngay dong
            var dsstkcongaydong = dal.GetList_SoTietKiem().Where(x => x.Ngaydongso != null);
            var list_SoTietKiemDong = dsstkcongaydong.Where(x => x.Ngaydongso!.Value.Month == thang && x.Ngaydongso!.Value.Year == nam && x.Loaitietkiem == loaitk)
                .GroupBy(x => x.Ngaydongso!.Value.Date)
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
            var list_Stt = Enumerable.Range(1, list_BaoCaoDongMoSoThang2.Count()).ToList();
            var list_BaoCaoDongMoSoThang3 = list_BaoCaoDongMoSoThang2.Select((x, index) => new { Stt = list_Stt[index], x.Ngay, x.Soluongmo, x.Soluongdong, x.Chenhlech });
            var result = list_BaoCaoDongMoSoThang3.ToList<dynamic>();
            return result;
        }

        public List<LoaiTietKiem> getList_LoaiTietKiem()
        {   
            DAL_Config dal = DAL_Config.Instance;
            var list_LoaiTietKiem = dal.GetList_LoaiTietKiem();
            return list_LoaiTietKiem;
        }

        public bool add_LoaiTietKiem(int kyHan, double laiSuat)
        {   
            bool result = true;
            //validate
            if (kyHan < 0)
            {
                result = false;
                throw new Exception("Kỳ hạn không hợp lệ");
            }
            DAL_Config dal = DAL_Config.Instance;
            LoaiTietKiem add = new LoaiTietKiem();
            add.Kyhan = kyHan;
            add.Laisuat = laiSuat;
            try
            {
                dal.Add_LoaiTietKiem(add);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

       
        public bool delete_LoaiTietKiem(LoaiTietKiem loaiTietKiem)
        {   
            bool result = true;
            DAL_Config dal = DAL_Config.Instance;
            try
            {
                dal.Delete_LoaiTietKiem(loaiTietKiem);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public bool update_LoaiTietKiem(int kyHan, double laiSuat)
        {   
            bool result = true;
            if (kyHan < 0)
            {
                result = false;
                throw new Exception("Kỳ hạn không hợp lệ");
            }
            DAL_Config dal = DAL_Config.Instance;
            LoaiTietKiem upd = new LoaiTietKiem();
            upd.Kyhan = kyHan;
            upd.Laisuat = laiSuat;
            try
            {
                dal.Update_LoaiTietKiem(upd);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;

        }

        public bool update_NgayGuiToiThieu(int ngay)
        {
            var result = true;
            if (ngay > 0)
            {
                   Config.Update_SoNgayGuiToiThieu(ngay);
            }
            else
            {
                result = false;
                throw new Exception("Ngày gửi tối thiểu không hợp lệ");
            }
            return result;
        }

        public bool update_SoTienGuiToiThieu(int soTien)
        {
            var result = true;

            if(soTien <= 0)
            {
                result = false;
                throw new Exception("Số tiền gửi tối thiểu không hợp lệ");
            }
            else
            {
                Config.Update_SoTienGuiToiThieu(soTien);
            }

            return result;
        }

        public int get_NgayGuiToiThieu()
        {
            return Config.Get_SoNgayGuiToiThieu();
        }

        public long get_SoTienGuiToiThieu()
        {
            return Config.Get_SoTienGuiToiThieu();
        }



    }

}
