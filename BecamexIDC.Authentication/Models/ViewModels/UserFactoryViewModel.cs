using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BecamexIDC.Authentication.Models.ViewModels
{
    public class UserFactoryViewModel
    {
        public string UserId { set; get; }
        public int FactoryId { get; set; }
        public string FactoryName { get; set; }
        public bool Active { get; set; }
    }
}
