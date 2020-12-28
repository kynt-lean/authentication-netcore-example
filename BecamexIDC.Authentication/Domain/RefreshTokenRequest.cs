namespace BecamexIDC.Authentication.Domain
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string Password { get; set; }
    }
}