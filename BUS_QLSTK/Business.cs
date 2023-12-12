
using DAL_QLSTK;
using DTO_QLSTK;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DAL_QLSTK;
using DTO_QLSTK;
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
        public bool create_SoTietKiem(int maSo, string CCCD, string tenKH, string diaChi, int kyHan, long soTien)
        {
            var soTietKiem = new SoTietKiem();
            soTietKiem.Maso = maSo;
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
                var phieuGui = new PhieuGui();
                phieuGui.Maso = soTietKiem.Maso;
                phieuGui.Ngaygui = DateTime.Now;
                phieuGui.Sotien = soTien;
                phieuGui.Maphieugui = getNew_MaPhieuGui();
                PhieuGui.Add_PhieuGui(phieuGui);
                KhachHang.Add_KhachHang(kh);
            }
              

            return result;  
        }


        public bool create_PhieuGui(int maSo, string CCCD, long soTien)
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
                phieuGui.Maphieugui = getNew_MaPhieuGui();

                PhieuGui.Add_PhieuGui(phieuGui);
            }

            return result;
        }

        public bool create_PhieuRut(int maSo, string CCCD, long? soTien)
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


            if (soTietKiem.Trangthai == 0)
            {
                result = false;
                throw new Exception("Sổ đã đóng");
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

                    var soThangGui = (DateTime.Now - latestDate)?.Days / 30;

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
                            soTietKiem.Ngaydongso = DateTime.Now;
                        }
                        SoTietKiem.Update_SoTietKiem(soTietKiem);

                        var phieuRut = new PhieuRut();
                        phieuRut.Maso = maSo;
                        phieuRut.Ngayrut = DateTime.Now;
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
                    soTietKiem.Ngaydongso = DateTime.Now;
                    SoTietKiem.Update_SoTietKiem(soTietKiem);

                    var phieuRut = new PhieuRut();
                    phieuRut.Maso = maSo;
                    phieuRut.Ngayrut = DateTime.Now;
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
            var max = list_SoTietKiem.Max(x => x.Maso);
            var res = max + 1;
            return res;
        }

        public int getNew_MaPhieuGui()
        {
            var list_PhieuGui = PhieuGui.GetList_PhieuGui();
            var max = list_PhieuGui.Max(x => x.Maphieugui);
            var res = max + 1;
            return res;
        }


        public int getNew_MaPhieuRut()
        {
            var list_PhieuRut = PhieuRut.GetList_PhieuRut();
            var max = list_PhieuRut.Max(x => x.Maphieurut);
            var res = max + 1;
            return res;
        }

        public List<dynamic> getList_SoTietKiem()
        {
            return new List<dynamic>();
        }

        //todo: đồng nhất kiểu Date
        public List<dynamic> getList_DoanhSoNgay(Date ngay)
        {
            return new List<dynamic>();
        }

        public List<dynamic> getList_BaoCaoDongMoSoThang(int thang, int nam)
        {
            return new List<dynamic>();
        }

        public List<LoaiTietKiem> getList_LoaiTietKiem()
        {
            return Config.GetList_LoaiTietKiem();
        }

        public bool add_LoaiTietKiem(int kyHan, double laiSuat)
        {
            var result = true;

            var listLoaiTietKiem = Config.GetList_LoaiTietKiem();

            foreach(var loaiTietKiem in listLoaiTietKiem)
            {
                if(loaiTietKiem.Kyhan == kyHan)
                {
                    result = false;
                    throw new Exception("Loại tiết kiệm đã tồn tại");
                }
            }

            if(result)
            {
                var loaiTietKiem = new LoaiTietKiem();
                loaiTietKiem.Kyhan = kyHan;
                loaiTietKiem.Laisuat = laiSuat;
                Config.Add_LoaiTietKiem(loaiTietKiem);
            }

            return result;
        }


        public bool delete_LoaiTietKiem(LoaiTietKiem loaiTietKiem)
        {
            var result = true;
            
            Config.Delete_LoaiTietKiem(loaiTietKiem);

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
