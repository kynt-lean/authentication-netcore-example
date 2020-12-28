using System.Collections;
using System.Collections.Generic;

namespace BecamexIDC.Authentication.Models
{
    public class UserToRole
    {
        public string Username { set; get; }

        public IEnumerable<string> roles { set; get; }
    }
}