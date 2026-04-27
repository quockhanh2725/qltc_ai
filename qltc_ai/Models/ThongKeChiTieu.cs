using System;

namespace qltc_ai.Models;

public partial class ThongKeChiTieu
{
    public int IdThongKe { get; set; }

    public int? IdNguoiDung { get; set; }

    public int? Thang { get; set; }

    public int? Nam { get; set; }

    public decimal? TongThu { get; set; }

    public decimal? TongChi { get; set; }

    public decimal? TongTietKiem { get; set; }

    public int? SoGiaoDich { get; set; }

    public string? DanhMucChiNhieuNhat { get; set; }

    public string? TrangThaiCanhBao { get; set; } 

    public string? MoTaCanhBao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual Nguoidung? IdNguoiDungNavigation { get; set; }
}
