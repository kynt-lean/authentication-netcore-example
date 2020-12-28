using System.Collections.Generic;

namespace BecamexIDC.Authentication.Models
{
    public class UserPolicyDto
    {
        public string userName { get; set; }
        public List<string> Policies { get; set; }
    }
}