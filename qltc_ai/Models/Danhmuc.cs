using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Danhmuc
{
    public int IdDanhMuc { get; set; }

    public string? TenDanhMuc { get; set; }

    public string? Mau { get; set; } 

    public string? LoaiDanhMuc { get; set; } 

    public virtual ICollection<ChiTietDanhMuc> ChiTietDanhMuc { get; set; } = new List<ChiTietDanhMuc>();
}
