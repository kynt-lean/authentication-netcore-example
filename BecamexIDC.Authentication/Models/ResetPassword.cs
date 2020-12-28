namespace BecamexIDC.Authentication.Models
{
    public class ResetPassword
    {
        public string UserName {set;get;}
        public string Email { set; get; }
        public string Password { set; get; }
        public string Token { set; get; }

    }
}