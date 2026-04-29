using System;
using System.Collections.Generic;

namespace qltc_ai.Models;
public partial class AiPhanTich
{
    public int IdPhanTich { get; set; }

    public int? IdTaiKhoan { get; set; }

    public string? LoaiPhanTich { get; set; } 

    public string? KetQua { get; set; } 

    public string? DeXuat { get; set; } 

    public decimal? DoTinCay { get; set; } 

    public DateTime? NgayTao { get; set; }

    public virtual Taikhoan? IdTaiKhoanNavigation { get; set; }
}
