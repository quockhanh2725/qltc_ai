using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Tinnhan
{
    public int IdTinNhan { get; set; }

    public int? IdTroChuyen { get; set; }

    public string? NguoiGui { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? ThoiGianGui { get; set; }

    public virtual Trochuyen? IdTroChuyenNavigation { get; set; }
}
