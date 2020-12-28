using System;

namespace BecamexIDC.Authentication.Models.ViewModels
{
    public class FunctionViewModel
    {
        public int Id { get; set; }
        public int? ModuleId { get; set; }
        public string  Code { get; set; }

        public int? ParentId { get; set; }
        public string NameVi { get; set; }
        public string NameEn { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public int? Status { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}