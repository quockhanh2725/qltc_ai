using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Thongbao
{
    public int IdThongBao { get; set; }

    public int? IdTaiKhoan { get; set; }

    public string? TieuDe { get; set; }

    public string? NoiDung { get; set; }

    public sbyte? IsRead { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual Taikhoan? IdTaiKhoanNavigation { get; set; }
}
