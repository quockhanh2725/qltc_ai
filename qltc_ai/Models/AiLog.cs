using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace qltc_ai.Models;

public partial class AiLog
{
    public int IdLog { get; set; }

    public int? IdTaiKhoan { get; set; }

    [Column(TypeName = "longtext")]
    public string? CauHoi { get; set; }

    [Column(TypeName = "longtext")]
    public string? TraLoi { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual Taikhoan? IdTaiKhoanNavigation { get; set; }
}
