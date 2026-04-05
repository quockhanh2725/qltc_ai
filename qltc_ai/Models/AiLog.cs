using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class AiLog
{
    public int IdLog { get; set; }

    public int? IdTaiKhoan { get; set; }

    public string? CauHoi { get; set; }

    public string? TraLoi { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual Taikhoan? IdTaiKhoanNavigation { get; set; }
}
