using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Ngansach
{
    public int IdNganSach { get; set; }

    public int? IdTaiKhoan { get; set; }

    public decimal? TongTien { get; set; }

    public DateTime? Thang { get; set; }

    public virtual Taikhoan? IdTaiKhoanNavigation { get; set; }
    public virtual ICollection<Danhmuc> Danhmuc { get; set; } = null!;
}
