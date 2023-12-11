using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLSTK;

namespace DAL_QLSTK
{
    public class DAL_SoTietKiem
    {
        private static DAL_SoTietKiem _Instance;
        private QlStkContext context = new QlStkContext();

        public static DAL_SoTietKiem Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DAL_SoTietKiem();
                }
                return _Instance;
            }
        }

        public List<SoTietKiem> GetList_SoTietKiem()
        {
            return context.SoTietKiems.ToList();
        }

        public void Add_SoTietKiem(SoTietKiem stk)
        {
            if (context.SoTietKiems.Find(stk.Maso) != null)
                throw new Exception("Sổ tiết kiệm đã tồn tại");

            context.SoTietKiems.Add(stk);
            context.SaveChanges();
        }

        public void Update_SoTietKiem(SoTietKiem stk)
        {
            SoTietKiem soTietKiem = context.SoTietKiems.Find(stk.Maso);
            if(soTietKiem == null)
                throw new Exception("Không tìm thấy sổ tiết kiệm");
            soTietKiem.Trangthai = stk.Trangthai;
            soTietKiem.Ngaydongso = stk.Ngaydongso;
            soTietKiem.Sodu = stk.Sodu;

            context.SaveChanges();
        }
    }

}
