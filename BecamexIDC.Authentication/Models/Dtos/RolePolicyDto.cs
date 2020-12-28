using System.Collections.Generic;

namespace BecamexIDC.Authentication.Models
{
    public class RolePolicyDto
    {
        public string roleName { get; set; }
        public List<string> Policies { get; set; }
    }
}