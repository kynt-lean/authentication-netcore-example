namespace BecamexIDC.Authentication.Models
{
    public class RegisterAccount
    {
        public string Username { set; get; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}