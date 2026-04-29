using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Giaodich
{
    public int IdGiaoDich { get; set; }

    public int? IdTaiKhoan { get; set; }

    public int? IdChiTiet { get; set; }

    public decimal? Tien { get; set; }

    public string? NoiDung { get; set; }

    public string? LoaiGiaoDich { get; set; }

    public DateTime? NgayGiaoDich { get; set; }

    public virtual ChiTietDanhMuc? IdChiTietNavigation { get; set; }

    public virtual Taikhoan? IdTaiKhoanNavigation { get; set; }
}