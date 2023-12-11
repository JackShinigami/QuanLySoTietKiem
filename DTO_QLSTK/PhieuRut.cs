using System;
using System.Collections.Generic;

namespace DTO_QLSTK;

public partial class PhieuRut
{
    public int Maphieurut { get; set; }

    public int? Maso { get; set; }

    public DateTime? Ngayrut { get; set; }

    public long? Sotien { get; set; }

    public virtual SoTietKiem? MasoNavigation { get; set; }
}
