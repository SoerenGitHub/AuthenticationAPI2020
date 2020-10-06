using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuthAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthAPI.Auth {
    public class TokenGenerator {
        private readonly IConfiguration _configuration;

        public TokenGenerator(IConfiguration configurations) {
            _configuration = configurations;
        }

        public async Task<Token> GenerateJwtTokenAsync(User trustedUser) {
            return await Task.Run(() => {
                try {
                    Token expToken = new Token{
                        ExpiryDate = (long)(DateTime.UtcNow.AddMinutes(10).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                        UserId = trustedUser.UserId,
                        IssuedAt = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                        User = trustedUser,
                        Value = Guid.NewGuid().ToString()
                    };

                    if(expToken == null) {
                        return null;
                    }

                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                    Claim[] claims = new Claim[] {
                        new Claim(JwtRegisteredClaimNames.Sub, Convert.ToString(trustedUser.UserId)),
                        new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]),
                        new Claim(JwtRegisteredClaimNames.Exp, expToken.ExpiryDate.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, expToken.Value),
                        new Claim(JwtRegisteredClaimNames.Iat, expToken.IssuedAt.ToString()),  
                        new Claim(ClaimTypes.Role, trustedUser.Role.ToString())
                    };

                    ClaimsIdentity identity = new ClaimsIdentity(claims);

                    RSAManager rSAManager = RSAManager.GetInstance;

                    SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor {
                        Subject = identity,
                        Expires = DateTime.UtcNow.AddMinutes(10),
                        SigningCredentials = new SigningCredentials(rSAManager.Key, SecurityAlgorithms.RsaSsaPssSha256)
                    };

                    SecurityToken token = tokenHandler.CreateToken(descriptor);
                    string jwtToken = tokenHandler.WriteToken(token);
                    if(jwtToken != null) {
                        expToken.JwtToken = jwtToken;
                        return expToken;
                    }
                }
                catch(Exception) {
                    return null;
                }
                return null;
            });
        }
    }
}