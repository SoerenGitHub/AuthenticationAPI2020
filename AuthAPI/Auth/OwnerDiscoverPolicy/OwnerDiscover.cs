

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AuthAPI.Auth {
    public class OwnerDiscover {
        public OwnerDiscover()
        {
            
        }

         public async Task<bool> isOwnerAsync (JwtSecurityToken jwtToken, HttpContext httpContext) {
            return await Task.Run(() => {
                Nullable<int> userIdFromToken = getUserIdWithToken(jwtToken);
                Nullable<int> userIdFromRequest = getRequestedUserIdWithHttpContext(httpContext);

                if(
                    userIdFromToken != null &&
                    userIdFromRequest != null &&
                    userIdFromToken == userIdFromRequest
                ) {
                    return true;
                }
                return false;
            });
        }

        private Nullable<int> getUserIdWithToken(JwtSecurityToken jwtToken) {
            try{
                string userIdString = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
                return int.Parse(userIdString);
            }
            catch(Exception) {
                return null;
            }
        }

        private Nullable<int> getRequestedUserIdWithHttpContext(HttpContext httpContext) {
            try{
                string requestedUserIdString = httpContext.Request.RouteValues["userId"]?.ToString();
                return int.Parse(requestedUserIdString);
            }
            catch(Exception) {
                return null;
            }
        }
    }
}