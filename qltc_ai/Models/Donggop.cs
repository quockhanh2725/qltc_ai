using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Donggop
{
    public int IdDongGop { get; set; }

    public int? IdMucTieu { get; set; }

    public decimal? SoTien { get; set; }

    public DateTime? NgayGop { get; set; }

    public virtual Muctieu? IdMucTieuNavigation { get; set; }
}
