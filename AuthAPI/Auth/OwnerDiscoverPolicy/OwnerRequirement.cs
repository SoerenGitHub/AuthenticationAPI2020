using Microsoft.AspNetCore.Authorization;

namespace AuthAPI.Auth {
    public class OwnerRequirement : IAuthorizationRequirement {
        public OwnerRequirement()
        {
            
        }
    }
}