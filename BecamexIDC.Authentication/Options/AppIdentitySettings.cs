namespace BecamexIDC.Authentication.Options
{
    public class UserSettings
    {
        public bool RequireUniqueEmail { get; set; }
    }
    public class WcfSettings
    {
        public string RemoteAddress { get; set; }
    }
    public class PasswordSettings
    {
        public int RequiredLength { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public int RequiredUniqueChars { get; set; }
    }
    public class LockoutSettings
    {
        public bool AllowedForNewUsers { get; set; }
        public int DefaultLockoutTimeSpanInMins { get; set; }
        public int MaxFailedAccessAttempts { get; set; }
    }
}