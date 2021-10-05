using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NavigusWebApi.Manager
{
    public class JwtAuthManager : IJwtAuthManager
    {
        //encryption key for JWT
        public string Key { get; set; }
        public JwtAuthManager(string key)
        {
            Key = key;
        }

        //Generate JWT Token
        public string Authenticate(string email,string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            //get bytes[] from encryption string
            byte[] tokenKey = Encoding.ASCII.GetBytes(Key);

            var tokenDescription = new SecurityTokenDescriptor()
            {
                //user email is used for claim
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Role,role)
                    }),

                //1 hour expiration
                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials
                (
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token=tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);

        }
    }
}
