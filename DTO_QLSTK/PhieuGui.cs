using System;
using System.Collections.Generic;

namespace DTO_QLSTK;

public partial class PhieuGui
{
    public int Maphieugui { get; set; }

    public int? Maso { get; set; }

    public DateTime? Ngaygui { get; set; }

    public long? Sotien { get; set; }

    public virtual SoTietKiem? MasoNavigation { get; set; }
}
