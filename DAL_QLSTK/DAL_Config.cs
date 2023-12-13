using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using DTO_QLSTK;

namespace DAL_QLSTK
{
    public class DAL_Config
    {
        private static DAL_Config _Instance;
        private QlStkContext context = new QlStkContext();

        private DAL_Config()
        {
        }

        public static DAL_Config Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DAL_Config();
                }
                return _Instance;
            }
        }

        public List<LoaiTietKiem> GetList_LoaiTietKiem()
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            return context.LoaiTietKiems.ToList();
        }

        public void Add_LoaiTietKiem(LoaiTietKiem ltk)
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            if (context.LoaiTietKiems.Find(ltk.Kyhan) != null)
                throw new Exception("Loại tiết kiệm đã tồn tại");

            context.LoaiTietKiems.Add(ltk);
            context.SaveChanges();
        }

        public void Delete_LoaiTietKiem(LoaiTietKiem ltk)
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            LoaiTietKiem loaiTietKiem = context.LoaiTietKiems.Find(ltk.Kyhan);
            if(loaiTietKiem == null)
                throw new Exception("Loại tiết kiệm không tồn tại");

            context.LoaiTietKiems.Remove(loaiTietKiem);
            context.SaveChanges();
        }

        public void Update_LoaiTietKiem(LoaiTietKiem ltk)
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            LoaiTietKiem loaiTietKiem = context.LoaiTietKiems.Find(ltk.Kyhan);
            if (loaiTietKiem == null)
                throw new Exception("Loại tiết kiệm không tồn tại");

            loaiTietKiem.Laisuat = ltk.Laisuat;
            context.SaveChanges();
        }

        public long Get_SoTienGuiToiThieu()
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            List<ConfigToithieu> configToithieus = context.ConfigToithieus.ToList();
            if (configToithieus.Count == 0)
                return 0;

            return (long)configToithieus[0].Sotiengui;
        }

        public void Update_SoTienGuiToiThieu(long sotiengui)
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            List<ConfigToithieu> configToithieus = context.ConfigToithieus.ToList();
            if (configToithieus.Count == 0)
            {
                ConfigToithieu configToithieu = new ConfigToithieu();
                configToithieu.Sotiengui = sotiengui;
                context.ConfigToithieus.Add(configToithieu);
            }
            else
            {
                context.ConfigToithieus.First().Sotiengui = sotiengui;
            }
            context.SaveChanges();
        }

        public int Get_SoNgayGuiToiThieu()
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            List<ConfigToithieu> configToithieus = context.ConfigToithieus.ToList();
            if (configToithieus.Count == 0)
                return 0;

            return (int)configToithieus[0].Ngaygui;
        }

        public void Update_SoNgayGuiToiThieu(int ngaygui)
        {
            if (!context.Database.CanConnect())
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu");

            List<ConfigToithieu> configToithieus = context.ConfigToithieus.ToList();
            if (configToithieus.Count == 0)
            {
                ConfigToithieu configToithieu = new ConfigToithieu();
                configToithieu.Ngaygui = ngaygui;
                context.ConfigToithieus.Add(configToithieu);
            }
            else
            {
                context.ConfigToithieus.First().Ngaygui = ngaygui;
            }
            context.SaveChanges();
        }
    }
}
