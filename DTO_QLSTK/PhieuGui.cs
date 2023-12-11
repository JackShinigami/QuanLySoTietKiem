using System;
using System.Collections.Generic;

namespace DTO_QLSTK;

public partial class PhieuGui
{
    public string Maphieugui { get; set; } = null!;

    public string? Maso { get; set; }

    public DateTime? Ngaygui { get; set; }

    public int? Sotien { get; set; }

    public virtual SoTietKiem? MasoNavigation { get; set; }
}
