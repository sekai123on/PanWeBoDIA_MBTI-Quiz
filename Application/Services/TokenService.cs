using MbtiApi.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

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
            // Implementation for creating JWT token
        }
    }
}
