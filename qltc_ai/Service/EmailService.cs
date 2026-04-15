using System.Net.Mail;
using System.Net;

namespace qltc_ai.Service
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendOTP(string toEmail, string otp)
        {
            var fromEmail = _config["EmailSettings:SenderEmail"];
            var password = _config["EmailSettings:Password"];

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Ma OTP",
                IsBodyHtml = true,
                Body = $@"
                    <p>Day la ma OTP cua ban, tuyet doi khong chia se voi nguoi khac.</p>
                    <p><b>Ma OTP cua ban la: {otp}</b></p>
                    <p>Ma OTP se het hieu luc sau 5 phut</p>
                "
            };

            smtp.Send(message);
        }
    }
}
