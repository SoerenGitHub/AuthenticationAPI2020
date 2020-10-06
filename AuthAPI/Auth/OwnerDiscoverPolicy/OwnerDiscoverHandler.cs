using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AuthAPI.Auth {
    public class OwnerDiscoverHandler : AuthorizationHandler<OwnerRequirement> {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public OwnerDiscoverHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        protected async override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OwnerRequirement requirement
        )
        {
            if(context.User.IsInRole(Roles.Admin.ToString())) {
                context.Succeed(requirement);
            }

            HttpContext httpContext = _httpContextAccessor.HttpContext;
            
            TokenParser tokenParser = new TokenParser(_configuration);
            JwtSecurityToken jwtToken = await tokenParser.ParseTokenFromContextAsync(httpContext);

            OwnerDiscover ownerDiscover = new OwnerDiscover();
            if(await ownerDiscover.isOwnerAsync(jwtToken, httpContext)) {
                context.Succeed(requirement);
            } 
            else {
                context.Fail();
            }
        }
    }
}