using BecamexIDC.Authentication.Domain;
using System.Net;

namespace BecamexIDC.Authentication.Helpers
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
        public eOfficeCookie Cookie { get; set; }
    }
}