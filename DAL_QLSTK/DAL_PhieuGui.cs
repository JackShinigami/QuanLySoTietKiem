using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLSTK;

namespace DAL_QLSTK
{
    public class DAL_PhieuGui
    {
        private static DAL_PhieuGui _Instance;
        private QlStkContext context = new QlStkContext();

        private DAL_PhieuGui()
        {
        }

        public static DAL_PhieuGui Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DAL_PhieuGui();
                }
                return _Instance;
            }
        }

        public List<PhieuGui> GetList_PhieuGui()
        {
            return context.PhieuGuis.ToList();
        }

        public void Add_PhieuGui(PhieuGui pg)
        {
            if (context.PhieuGuis.Find(pg.Maphieugui) != null)
                throw new Exception("Phiếu gửi đã tồn tại");

            context.PhieuGuis.Add(pg);
            context.SaveChanges();
        }
    }
}
