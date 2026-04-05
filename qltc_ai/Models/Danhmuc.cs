using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Danhmuc
{
    public int IdDanhMuc { get; set; }

    public int? IdTongTien { get; set; }

    public string? TenDanhMuc { get; set; }

    public string? Mau { get; set; }

    public decimal? GioiHanTien { get; set; }

    public decimal? TienDaTieu { get; set; }

    public string? DanhGia { get; set; }

    public virtual ICollection<Giaodich> Giaodiches { get; set; } = new List<Giaodich>();
}
