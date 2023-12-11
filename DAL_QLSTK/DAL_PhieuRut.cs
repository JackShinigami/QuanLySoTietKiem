using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLSTK;

namespace DAL_QLSTK
{
    public class DAL_PhieuRut
    {
        private static DAL_PhieuRut _Instance;
        private QlStkContext context = new QlStkContext();
        
        public static DAL_PhieuRut Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DAL_PhieuRut();
                }
                return _Instance;
            }
        }

        public List<PhieuRut> GetList_PhieuRut()
        {
            return context.PhieuRuts.ToList();
        }

        public void Add_PhieuRut(PhieuRut pr)
        {
            if (context.PhieuRuts.Find(pr.Maphieurut) != null)
                throw new Exception("Phiếu rút đã tồn tại");

            context.PhieuRuts.Add(pr);
            context.SaveChanges();
        }
    }
}
