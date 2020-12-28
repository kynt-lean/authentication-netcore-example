namespace BecamexIDC.Authentication.Models.ViewModels
{
    public class PermissionViewModel
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string FunctionCode { get; set; }
        public string ModuleName { get; set; }
        public int ModuleId { get; set; }
        public bool ModulePermission { get; set; }
        public string FunctionName { get; set; }
        public string ParentId { get; set; }
        public bool CanCreate { set; get; }
        public bool CanRead { set; get; }
        public bool CanUpdate { set; get; }
        public bool CanDelete { set; get; }
        public bool CheckRow { set; get; }
    }
}