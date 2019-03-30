using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using mvcCookieAuthSample.Models;

namespace mvcCookieAuthSample.Services
{
    public class ProfileService : IProfileService
    {
        public UserManager<ApplicationUser> _userManager;

        //public RoleManager<ApplicationUserRole> _userRoleManageer;
        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        private async Task<List<Claim>> GetClaimsFormUserAsync(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
            };
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }

            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                claims.Add(new Claim("avatar", user.Avatar));
            }
            return claims;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectid = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            var user =await _userManager.FindByIdAsync(subjectid);
            var claims =await GetClaimsFormUserAsync(user);
            context.IssuedClaims = claims;

        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = false;

          var subjectid=  context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
          var user =await _userManager.FindByIdAsync(subjectid);
          context.IsActive = user!=null;


        }
    }
}
