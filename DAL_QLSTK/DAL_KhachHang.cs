using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using DTO_QLSTK;
namespace DAL_QLSTK
{
    public class DAL_KhachHang
    {
        private static DAL_KhachHang _Instance;
        private QlStkContext context = new QlStkContext();

        private DAL_KhachHang()
        {
        }

        public static DAL_KhachHang Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DAL_KhachHang();
                }
                return _Instance;
            }
        }
        
        public List<KhachHang> GetList_KhachHang()
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            return context.KhachHangs.ToList();
        }

        public void Add_KhachHang(KhachHang kh)
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            if (context.KhachHangs.Find(kh.Cccd) != null)
                throw new Exception("Khách hàng đã tồn tại");
            context.KhachHangs.Add(kh);
            context.SaveChanges();
        }

        public KhachHang Find_KhachHang(string cccd)
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            return context.KhachHangs.Find(cccd);
        }
    }
}
