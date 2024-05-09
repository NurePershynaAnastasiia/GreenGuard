using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GreenGuard.Dto;
using Microsoft.IdentityModel.Tokens;

namespace GreenGuard.Services
{
    public class JwtTokenService
    {
        private readonly string _key;

        public JwtTokenService(IConfiguration config)
        {
            _key = config["Jwt:Key"];
        }

        public string GenerateToken(WorkerDto worker)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, worker.WorkerName),
                new Claim(ClaimTypes.Email, worker.Email),
                new Claim(ClaimTypes.NameIdentifier, worker.WorkerId.ToString()),
                new Claim(ClaimTypes.Role, (worker.IsAdmin == null || !(bool)worker.IsAdmin)? "user" :  "admin")
                }),
                Expires = DateTime.UtcNow.AddDays(7), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
