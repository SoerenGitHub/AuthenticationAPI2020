using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AuthAPI.Models;

namespace AuthAPI.Auth {
    public class TokenValidator {
        private readonly JwtSecurityToken _jwtToken;
        private readonly Token _trustedToken;
        private readonly User _trustedUser;

        public TokenValidator(JwtSecurityToken jwtToken, Token trustedToken, User trustedUser)
        {
            _jwtToken = jwtToken;
            _trustedToken = trustedToken;
            _trustedUser = trustedUser;
        }

        public async Task<bool> HasValidPayloadAsync() {
            return await Task.Run(() => {
                return (
                    _jwtToken != null &&
                    _trustedToken != null &&
                    _trustedUser != null &&
                    IsUserIdValid() &&
                    IsRoleValid() &&
                    IsExpiryDateValid() &&
                    IsTokenValueValid()
                );
            });
        }

        private bool IsUserIdValid() {
            try{
                int userId = -1;
                string userIdString = _jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
                userId = int.Parse(userIdString);
                return (userId == _trustedUser.UserId && _trustedToken.UserId == userId);
            }
            catch(Exception) {
                return false;
            }
        }

        private bool IsRoleValid() {
            try{
                string role = _jwtToken.Claims.First(x => x.Type == "role")?.Value;
                return (role == _trustedUser.Role.ToString());
            }
            catch(Exception) {
                return false;
            }
        }

        private bool IsExpiryDateValid() {
             try{
                long expiryDate = -1;
                string ExpiryDateString = _jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value;
                expiryDate = long.Parse(ExpiryDateString);
                return (expiryDate == _trustedToken.ExpiryDate);
            }
            catch(Exception) {
                return false;
            }
        }

        private bool IsTokenValueValid() {
            try{
                string tokenValue = _jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
                return (tokenValue == _trustedToken.Value);
            }
            catch(Exception) {
                return false;
            }
        }
    }
}