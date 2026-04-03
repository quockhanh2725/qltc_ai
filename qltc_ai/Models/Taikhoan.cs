using System;
using System.Collections.Generic;

namespace qltc_ai.Models;

public partial class Taikhoan
{
    public int IdTaiKhoan { get; set; }

    public int? RoleId { get; set; }

    public string? Email { get; set; }

    public string? MatKhau { get; set; }

    public sbyte? IsActive { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<AiLog> AiLog { get; set; } = new List<AiLog>();

    public virtual ICollection<Giaodich> Giaodich { get; set; } = new List<Giaodich>();

    public virtual ICollection<Muctieu> Muctieu { get; set; } = new List<Muctieu>();

    public virtual ICollection<Ngansach> Ngansach { get; set; } = new List<Ngansach>();

    public virtual Nguoidung? Nguoidung { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<Thongbao> Thongbao { get; set; } = new List<Thongbao>();

    public virtual ICollection<Trochuyen> Trochuyen { get; set; } = new List<Trochuyen>();
}
