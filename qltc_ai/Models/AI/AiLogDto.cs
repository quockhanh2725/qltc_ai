namespace qltc_ai.Models.AI
{
    public class AiLogDto
    {
        public int IdLog { get; set; }
        public int IdTaiKhoan { get; set; }
        public string? TenNguoiDung { get; set; }
        public string? CauHoi { get; set; }
        public string? TraLoi { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}
