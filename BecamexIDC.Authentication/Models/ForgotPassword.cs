using System.ComponentModel.DataAnnotations;

namespace BecamexIDC.Authentication.Models
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email {set;get;}
    }
}