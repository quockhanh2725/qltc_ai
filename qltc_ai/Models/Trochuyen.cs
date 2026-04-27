using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Trochuyen
{
    public int IdTroChuyen { get; set; }

    public int? IdTaiKhoan { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual Taikhoan? IdTaiKhoanNavigation { get; set; }

    public virtual ICollection<Tinnhan> Tinnhan { get; set; } = new List<Tinnhan>();
}
