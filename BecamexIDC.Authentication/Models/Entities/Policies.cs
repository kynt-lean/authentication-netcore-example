using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BecamexIDC.Authentication.Models
{
    public class Policies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }        
        public string Type { get; set; }
        public string Policy { get; set; }
        public string Description_EN { get; set; }
        public string Description_VN { get; set; }
    }
}