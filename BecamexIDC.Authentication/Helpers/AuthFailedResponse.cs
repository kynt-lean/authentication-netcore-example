using System.Collections.Generic;

namespace BecamexIDC.Authentication.Helpers
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}