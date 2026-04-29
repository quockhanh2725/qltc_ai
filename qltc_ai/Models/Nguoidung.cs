using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Nguoidung
{
    public int IdNguoiDung { get; set; }

    public int? IdTaiKhoan { get; set; }

    public string? TenNguoiDung { get; set; }

    public virtual Taikhoan? IdTaiKhoanNavigation { get; set; }

    public virtual ICollection<ThongKeChiTieu> ThongKeChiTieu { get; set; } = new List<ThongKeChiTieu>();
}
