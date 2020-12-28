namespace BecamexIDC.Authentication.Models
{
     public class Login
    {
        public string Username { set; get; }
        public string Email { set; get; }
        public string Password { set; get; }
        public bool RememberMe { set; get; }
    }
}