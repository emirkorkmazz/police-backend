using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Police.Data;
using Police.Helpers;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;


namespace Police.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email);

            if (user == null)
                return Unauthorized("Geçersiz e-posta veya şifre.");

            if (!user.Status)
                return Unauthorized("Yöneticiniz ile iletişime geçiniz.");

            if (DateTime.UtcNow > user.LicenseDate)
                return Unauthorized("Lisans süreniz dolmuştur.");

            if (user.BanEndTime.HasValue && user.BanEndTime > DateTime.UtcNow)
                return Unauthorized($"Hatalı giriş nedeniyle {user.BanEndTime.Value.Subtract(DateTime.UtcNow).TotalMinutes:F0} dakika bekleyin.");

            var passwordHasher = new PasswordHasher();
            if (!passwordHasher.VerifyPassword(user.HashedPassword, password))
            {
                user.BanCount++;
                if (user.BanCount >= 3)
                {
                    user.BanReason = "3 kez yanlış şifre girişi yapıldı. 5 dakika sonra tekrar deneyiniz.";
                    user.BanEndTime = DateTime.UtcNow.AddMinutes(5);
                    user.BanCount = 0;
                }
                else
                {
                    user.BanReason = "Geçersiz e-posta veya şifre.";
                }

                _context.SaveChanges();
                return Unauthorized(user.BanReason);
            }


            // Kullanıcı bilgilerini günceller
            user.BanCount = 0; // Ban sayacı sıfırlanır
            user.BanEndTime = null; // Ban süresi sıfırlanır
            user.BanReason = "Not specified"; // Ban sebebini 'Not specified' olarak ayarla
            _context.SaveChanges();

            return Ok(new { Token = user.Token, UserId = user.UserId });
        }


        [HttpPost("register")]
            public IActionResult Register(string email, string password)
            {
                var existingUser = _context.Users.SingleOrDefault(u => u.Email == email);

                if (existingUser != null)
                    return BadRequest("Bu e-posta adresi zaten kayıtlı.");

                var passwordHasher = new PasswordHasher();
                var hashedPassword = passwordHasher.HashPassword(password);

                var newUser = new UserModel
                {
                    UserName = email.Split('@')[0],
                    PhoneNumber = "Not specified",
                    UserType = "User",
                    Email = email,
                    HashedPassword = hashedPassword,
                    Status = true, 
                    LicenseDate = DateTime.UtcNow.AddYears(1),
                    BanCount = 0,
                    BanReason = "Not specified",
                    BanEndTime = null,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    Token = GenerateRandomToken(32) 
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return Ok("Kullanıcı başarıyla oluşturuldu.");
            }

            private string GenerateRandomToken(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                using (var crypto = new RNGCryptoServiceProvider())
                {
                    var data = new byte[length];
                    var result = new char[length];
                    crypto.GetBytes(data);
                    for (int i = 0; i < data.Length; i++)
                    {
                        var rnd = data[i] % chars.Length;
                        result[i] = chars[rnd];
                    }
                    return new string(result);
                }
            }


            [HttpPost("change-password")]
                public IActionResult ChangePassword(int userId, string token, string oldPassword, string newPassword)
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserId == userId);

                    if (user == null)
                        return NotFound("Kullanıcı bulunamadı.");

                    if (user.Token != token)
                        return Unauthorized("Geçersiz token.");

                    var passwordHasher = new PasswordHasher();
                    if (!passwordHasher.VerifyPassword(user.HashedPassword, oldPassword))
                        return Unauthorized("Mevcut şifre yanlış.");

                    user.HashedPassword = passwordHasher.HashPassword(newPassword);

                    user.UpdatedDate = DateTime.UtcNow;
                    _context.SaveChanges();

                    return Ok("Şifre başarıyla değiştirildi.");
                }

        [HttpPost("update-user")]
        public IActionResult UpdateUser(int userId, string token, UserModel updatedUser)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserId == userId);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            if (user.Token != token)
                return Unauthorized("Geçersiz token.");

            if (!string.IsNullOrEmpty(updatedUser.UserName))
                user.UserName = updatedUser.UserName;

            if (!string.IsNullOrEmpty(updatedUser.Name))
                user.Name = updatedUser.Name;

            if (!string.IsNullOrEmpty(updatedUser.Surname))
                user.Surname = updatedUser.Surname;

            if (!string.IsNullOrEmpty(updatedUser.HashedUserIdentityNo))
                user.HashedUserIdentityNo = updatedUser.HashedUserIdentityNo;

            if (!string.IsNullOrEmpty(updatedUser.Email))
                user.Email = updatedUser.Email;

            if (!string.IsNullOrEmpty(updatedUser.PhoneNumber))
                user.PhoneNumber = updatedUser.PhoneNumber;

            if (!string.IsNullOrEmpty(updatedUser.CompanyName))
                user.CompanyName = updatedUser.CompanyName;

            if (!string.IsNullOrEmpty(updatedUser.CompanyAddress))
                user.CompanyAddress = updatedUser.CompanyAddress;

            if (!string.IsNullOrEmpty(updatedUser.CompanyPhone))
                user.CompanyPhone = updatedUser.CompanyPhone;

            if (!string.IsNullOrEmpty(updatedUser.CompanyIdsJson))
                user.CompanyIdsJson = updatedUser.CompanyIdsJson;

            if (updatedUser.Status != user.Status)
                user.Status = updatedUser.Status;

            user.UpdatedDate = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok("Kullanıcı başarıyla güncellendi.");
        }

        [HttpPost("update-company-ids")]
        public IActionResult UpdateCompanyIds(int userId, string token, string newCompanyIdsJson)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserId == userId);

            if (user.Token != token)
                return Unauthorized("Geçersiz token.");

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            user.CompanyIdsJson = newCompanyIdsJson;
            user.UpdatedDate = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok("companyIdsJson başarıyla güncellendi.");
        }

        [HttpGet("get-company-ids")]
        public IActionResult GetCompanyIds(int userId, string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserId == userId);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            if (user.Token != token)
                return Unauthorized("Geçersiz token.");

            return Ok(new { CompanyIdsJson = user.CompanyIdsJson });
        }

        [HttpGet("user-working-insurance")]
        public IActionResult UserWorkingInsurance(int userId, string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserId == userId && u.Token == token);

            if (user == null)
                return Unauthorized("Geçersiz token veya kullanıcı.");

            var companyIdsJson = user.CompanyIdsJson;

            if (string.IsNullOrEmpty(companyIdsJson))
                return BadRequest("Kullanıcının çalıştığı sigorta şirketleri bulunamadı.");

            var companyIds = companyIdsJson.Split(',').Select(id => int.Parse(id)).ToList();

            var companyData = _context.Companies
                .Where(c => companyIds.Contains(c.CompanyId))
                .Select(c => new
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.FullName
                })
                .ToList();

            return Ok(companyData);
        }

        [HttpGet("user-info")]
        public IActionResult UserInfo(int userId, string token) 
        { 
            var user = _context.Users.SingleOrDefault(u => u.UserId == userId && u.Token == token); 
            if (user == null) 
                return Unauthorized("Geçersiz token veya kullanıcı."); 
            return Ok(user);
        }


    }
}
