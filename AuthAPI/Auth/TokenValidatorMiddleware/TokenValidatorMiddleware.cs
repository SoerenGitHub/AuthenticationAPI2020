using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AuthAPI.Auth 
{
    public class TokenValidatorMiddleware {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TokenValidatorMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepo userRepository, ITokenRepo tokenRepository)
        {
            if(await IsAnonymousAllowedAsync(context)){
                await _next(context);
            }
            else  {
                try
                {
                    TokenParser tokenParser = new TokenParser(_configuration);
                    JwtSecurityToken jwtToken = await tokenParser.ParseTokenFromContextAsync(context);

                    int userId = -1;
                    string userIdString = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
                    userId = int.Parse(userIdString);

                    User truestedUser = await userRepository.GetUserByIdAsync(userId);
                    Token trustedToken = await tokenRepository.GetTokenByUserIdAsync(userId);

                    TokenValidator tokenValidator = new TokenValidator(jwtToken, trustedToken, truestedUser);
                    if(await tokenValidator.HasValidPayloadAsync()) {
                        await _next(context);
                    } else {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    }
                }
                catch(Exception)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }
            }
        }

        private async Task<bool> IsAnonymousAllowedAsync(HttpContext context) {
            return await Task.Run(() => {
                var endpoint = context.GetEndpoint();
                if( endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object){
                    return true;
                }
                return false;
            });
        }
    }
}