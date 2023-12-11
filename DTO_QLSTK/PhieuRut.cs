using System;
using System.Collections.Generic;

namespace DTO_QLSTK;

public partial class PhieuRut
{
    public string Maphieurut { get; set; } = null!;

    public string? Maso { get; set; }

    public DateTime? Ngayrut { get; set; }

    public int? Sotien { get; set; }

    public virtual SoTietKiem? MasoNavigation { get; set; }
}
