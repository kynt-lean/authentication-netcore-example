using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BecamexIDC.Authentication.Models
{
    public class EOfficeUserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Manager { get; set; }
        public string DepartmentManager { get; set; }
        public string StaffId { get; set; }
        public string Position { get; set; }
        public bool? Gender { get; set; }
        public int? AfterCompletedDate { get; set; }
        public string DayOfHire { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string PersonId { get; set; }
        public string Department { get; set; }
        public string SiteName { get; set; }
        public string Modified { get; set; }
        public string Editor { get; set; }
        public string Created { get; set; }
        public string Author { get; set; }
    }
}