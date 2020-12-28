using System.Collections.Generic;
using BecamexIDC.Authentication.Models.ViewModels;

namespace BecamexIDC.Authentication.Models.Dtos
{
    public class PermissionDto
    {
         public List<PermissionViewModel> listPermmission { get; set; }
        public string roleId { get; set; }
    }

    public class CheckPermissionDto{
        public string functionCode{ get; set; }
        public string action{ get; set; }
        public  string[] roles{ get; set; }
    }
}