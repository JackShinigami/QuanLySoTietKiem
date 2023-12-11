using System;
using System.Collections.Generic;

namespace DTO_QLSTK;

public partial class KhachHang
{
    public string Cccd { get; set; } = null!;

    public string? Hoten { get; set; }

    public string? Diachi { get; set; }

    public virtual ICollection<SoTietKiem> SoTietKiems { get; set; } = new List<SoTietKiem>();
}
