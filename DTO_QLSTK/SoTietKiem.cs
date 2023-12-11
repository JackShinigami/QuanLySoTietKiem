using System;
using System.Collections.Generic;

namespace DTO_QLSTK;

public partial class SoTietKiem
{
    public string Maso { get; set; } = null!;

    public string? Cccd { get; set; }

    public DateTime? Ngaymoso { get; set; }

    public DateTime? Ngaydongso { get; set; }

    public int? Sodu { get; set; }

    public int? Loaitietkiem { get; set; }

    public int? Trangthai { get; set; }

    public double? Laisuat { get; set; }

    public int? Songayduocrut { get; set; }

    public int? Tienguitoithieu { get; set; }

    public virtual KhachHang? CccdNavigation { get; set; }

    public virtual ICollection<PhieuGui> PhieuGuis { get; set; } = new List<PhieuGui>();

    public virtual ICollection<PhieuRut> PhieuRuts { get; set; } = new List<PhieuRut>();
}
