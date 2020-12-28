using System;
using System.Collections.Generic;
using System.Net;

namespace BecamexIDC.Authentication.Domain
{
    public class AuthenticationResult
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public eOfficeCookie Cookie { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
    public class eOfficeCookie
    {
        public DateTime Expires { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Value_Cookie { get; set; }
        public string Domain { get; set; }        
    }
}