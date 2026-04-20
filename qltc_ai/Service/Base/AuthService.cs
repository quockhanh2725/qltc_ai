using Org.BouncyCastle.Crypto.Generators;
using qltc_ai.Models;
using qltc_ai.Models.Enum;
using qltc_ai.Repositories;

namespace qltc_ai.Service.Base
{
    public class AuthService : IAuthService
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IAccountRepository _repo;
        private readonly EmailService _emailService;

        public AuthService(IAccountService accountService, IUserService userService , IAccountRepository repo, EmailService emailService)
        {
            _accountService = accountService;
            _userService = userService;
            _repo = repo;
            _emailService = emailService;
        }

        public bool IsEmailExists(string email)
        {
            return _repo.IsEmailTaken(email);
        }

        public RegisterResult Register(string email)
        {
            if (!OtpStore.data.ContainsKey(email))
                return new RegisterResult { Status = RegisterStatus.NotSendOtp };

            var data = OtpStore.data[email];

            if (!data.IsVerified)
                return new RegisterResult { Status = RegisterStatus.NotVerified };

            var acc = _accountService.CreateAccount(new Taikhoan
            {
                Email = data.Email,
                MatKhau = data.Password
            });

            _userService.CreateUser(acc.IdTaiKhoan, data.Email);

            OtpStore.data.Remove(email);

            return new RegisterResult
            {
                Status = RegisterStatus.Success,
                Data = acc
            };
        }

        public void SaveOtp(string email, string password)
        {
            if (OtpStore.data.ContainsKey(email))
            {
                var old = OtpStore.data[email];

                if (old.NextSend > DateTime.Now)
                {
                    throw new Exception($"Đợi {(int)(old.NextSend - DateTime.Now).TotalSeconds}s để gửi lại");
                }
            }

            var otp = new Random().Next(100000, 999999).ToString();

            OtpStore.data[email] = new Otp
            {
                Code = otp,
                Expire = DateTime.Now.AddMinutes(5),
                IsVerified = false,
                NextSend = DateTime.Now.AddSeconds(60),
                Email = email,
                Password = password
            };
            Task.Run(() => _emailService.SendOTP(email, otp));
        }

        public bool VerifyOtp(string email, string otp)
        {
            if (!OtpStore.data.ContainsKey(email))
                return false;

            var data = OtpStore.data[email];

            if (data.Expire < DateTime.Now)
            {
                OtpStore.data.Remove(email);
                return false;
            }

            if (data.Code != otp)
                return false;

            data.IsVerified = true;
            return true;
        }
    }
}
