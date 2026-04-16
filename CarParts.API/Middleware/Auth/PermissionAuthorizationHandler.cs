using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CarParts.API.Middleware.Authentication
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PrivilegeRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PrivilegeRequirement requirement)
        {
            var claimsPrincipal = context.User;
            if (!AreClaimsValid(claimsPrincipal))
            {
                context.Fail();
                await Task.CompletedTask;
                return;
            }

            var userClaimsModel = ExtractUserClaims(claimsPrincipal);
            if (userClaimsModel.ClaimRoles == null)
            {
                context.Fail();
            }
            else
            {
                ValidateUserPrivileges(context, requirement, userClaimsModel.ClaimRoles);
            }

            await Task.CompletedTask;
        }

        private void ValidateUserPrivileges(AuthorizationHandlerContext context, PrivilegeRequirement requirement, IEnumerable<string> userClaimRoles)
        {
            if (requirement.Role == Policies.All)
            {
                if (userClaimRoles.Contains(Policies.User) || userClaimRoles.Contains(Policies.Admin))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            if (userClaimRoles.Contains(requirement.Role))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }

        private bool AreClaimsValid(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal != null &&
                claimsPrincipal.Identity != null &&
                claimsPrincipal.Identity.IsAuthenticated &&
                claimsPrincipal.HasClaim(claim => claim.Type.Equals(ClaimTypes.Role)) &&
                claimsPrincipal.HasClaim(claim => claim.Type.Equals(JwtRegisteredClaimNames.Sub));
        }

        private UserClaimModel ExtractUserClaims(ClaimsPrincipal claimsPrincipal)
        {
            var claimRoleValues = claimsPrincipal
                .FindAll(claim => claim.Type.Equals(ClaimTypes.Role))
                .Select(claim => claim.Value);

            var emailSub = claimsPrincipal
                .FindFirst(claim => claim.Type.Equals(JwtRegisteredClaimNames.Sub))?.Value;

            return new UserClaimModel
            {
                ClaimRoles = claimRoleValues,
                ClaimEmail = emailSub!
            };
        }
    }
}
