namespace qltc_ai.Models
{
    public class Otp
    {
        public string? Code { get; set; }
        public DateTime Expire { get; set; }
        public bool IsVerified { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
