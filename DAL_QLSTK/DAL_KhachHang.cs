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
            return context.KhachHangs.ToList();
        }

        public void Add_KhachHang(KhachHang kh)
        {
            if(context.KhachHangs.Find(kh.Cccd) != null)
                throw new Exception("Khách hàng đã tồn tại");
            context.KhachHangs.Add(kh);
            context.SaveChanges();
        }

        public KhachHang Find_KhachHang(string cccd)
        { 
            return context.KhachHangs.Find(cccd);
        }
    }
}
