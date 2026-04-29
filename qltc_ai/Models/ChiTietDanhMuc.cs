using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class ChiTietDanhMuc
{
    public int IdChiTiet { get; set; }

    public int IdDanhMuc { get; set; }

    public int? IdNganSach { get; set; }

    public decimal? GioiHanTien { get; set; }

    public decimal? TienDaTieu { get; set; }

    public string? DanhGia { get; set; }

    public virtual Danhmuc IdDanhMucNavigation { get; set; } = null!;

    public virtual Ngansach? IdNganSachNavigation { get; set; }

    public virtual ICollection<Giaodich> Giaodich { get; set; } = new List<Giaodich>();
}