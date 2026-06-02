using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace qltc_ai.Models;

public partial class Tinnhan
{
    public int IdTinNhan { get; set; }

    public int? IdTroChuyen { get; set; }

    public string? NguoiGui { get; set; }

    [Column(TypeName = "longtext")]
    public string? NoiDung { get; set; }

    public DateTime? ThoiGianGui { get; set; }

    public virtual Trochuyen? IdTroChuyenNavigation { get; set; }
}
