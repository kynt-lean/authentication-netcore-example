using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BecamexIDC.Authentication.Models
{
    public class Permissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }        
        public string RoleId { get; set; }

        public string FunctionCode { get; set; }

        public bool CanCreate { set; get; }
        public bool CanRead { set; get; }

        public bool CanUpdate { set; get; }
        public bool CanDelete { set; get; }

    }
}