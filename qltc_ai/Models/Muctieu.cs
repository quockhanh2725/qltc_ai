using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Muctieu
{
    public int IdMucTieu { get; set; }

    public int? IdTaiKhoan { get; set; }

    public string? TenMucTieu { get; set; }

    public decimal? TienMucTieu { get; set; }

    public DateTime? ThoiGianMucTieu { get; set; }

    public string? TrangThai { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<Donggop> Donggops { get; set; } = new List<Donggop>();

    public virtual Taikhoan? IdTaiKhoanNavigation { get; set; }
}
