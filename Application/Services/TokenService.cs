using MbtiApi.Application.Interfaces;
using MbtiApi.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MbtiApi.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;


        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        }

        public string CreateToken(User user)
        {
            // Token ထဲမှာ သယ်သွားမယ့် User ရဲ့ အချက်အလက် (Claims) တွေ သတ်မှတ်တာပါ
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };
            // HmacSha512 algorithm သုံးပြီး Key ကို Sign လုပ်မယ်
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Token ရဲ့ သက်တမ်းနဲ့ သတ်မှတ်ချက်တွေကို ထည့်သွင်းပုံဖော်တာပါ
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // Token သက်တမ်းကို ၇ ရက် ပေးထားတယ်
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // စာသား Token အဖြစ် ပြောင်းပြီး Return ပြန်ပေးမယ်
            return tokenHandler.WriteToken(token);
        }
    }
}
