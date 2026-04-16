namespace CarParts.API.Middleware.Authentication
{
    public class UserClaimModel
    {
        public string ClaimEmail { get; set; } = string.Empty;
        public IEnumerable<string> ClaimRoles { get; set; } = [];
    }
}
