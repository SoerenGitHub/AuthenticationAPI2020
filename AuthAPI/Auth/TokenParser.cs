using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthAPI.Auth {
    public class TokenParser {
        private readonly IConfiguration _configuration;

        public TokenParser(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<JwtSecurityToken> ParseTokenFromContextAsync(HttpContext context) {
            return await Task.Run(() => {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if( token != null) {
                    try
                    {
                        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                        RSAManager rSAManager = RSAManager.GetInstance;

                        tokenHandler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new RsaSecurityKey(rSAManager.Key.Rsa.ExportParameters(false)),
                            ValidateIssuer = true,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidIssuer = _configuration["Jwt:Issuer"],
                            ClockSkew = TimeSpan.Zero
                        }, out SecurityToken validatedToken);

                        return (JwtSecurityToken)validatedToken;
                    }
                    catch(Exception) {
                        return null;
                    }
                }
                return null;
            });
        }


    }
}