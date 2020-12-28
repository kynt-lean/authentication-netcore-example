namespace BecamexIDC.Authentication.Routes
{
    public static class ApiRoutes
    {
        public const string Base = Root;
        public const string Root = "auth";
        public const string Version = "v1";
        public static class Identity
        {
            public const string ForgotPassword = Base + "/identity/forgotPassword";
            public const string GetInfoSharePoint = Base + "/identity/getInfoSharePoint";
            public const string LDAP = Base + "/identity/ldap";
            public const string LDAPLogin = Base + "/identity/ldapLogin";
            public const string LogOut = Base + "/identity/logOut";
            public const string Login = Base + "/identity/login";
            public const string LoginSharePoint = Base + "/identity/loginSharePoint";
            public const string Refresh = Base + "/identity/refresh";
            public const string Register = Base + "/identity/register";
            public const string ResetPassword = Base + "/identity/resetPassword";
            public const string VirtualRefeshToken = Base + "/identity/VirtualRefeshToken";
            public const string GetEOfficeUserInfo = Base + "/identity/getEOfficeUserInfo";
            public const string GetBCMAppUserInfo = Base + "/identity/getBCMAppUserInfo";
            public const string GetEofficeUserNameOnly = Base + "/identity/getEofficeUserNameOnly";
            public const string GetEOfficeAccountsSharePoint = Base + "/identity/getEOfficeAccountsSharePoint";
        }
        public static class Role
        {
           

            public const string AddToRole = Base + "/role/addToRole";
            public const string CreateRole = Base + "/role/createRole";
            public const string GetRoles = Base + "/role/getRoles";
            public const string GetRolesAsync = Base + "/role/getRolesAsync";
            public const string GetRolesByUser = Base + "/role/getRolesByUser";
            public const string GetRolesDataGridPagination = Base + "/role/getRolesDataGridPagination";
            public const string GetUsersInRoleAsync = Base + "/role/getUsersInRoleAsync";        
            public const string RemoveToRole = Base + "/role/removeToRole";

        }
        public static class Policy
        {
            public const string AddPoliciesToRoles = Base + "/policy/addPoliciesToRoles"; //AddPoliciesToRoles
            public const string AddPoliciesToUser = Base + "/policy/AddPoliciesToUser"; //AddPoliciesToUser
            public const string AddPolicy = Base + "/policy/addPolicy";
            public const string AddRangePolicies = Base + "/policy/addRangePolicies";
            public const string GetAllPolicies = Base + "/policy/getAllPolicies"; //GetAllPolicies
            public const string GetPoliciesInRoleAsync = Base + "/policy/getPoliciesInRoleAsync";
            public const string GetPoliciesInUser = Base + "/policy/getPoliciesInUser"; //
            public const string RemovePoliciesToRoles = Base + "/policy/removePoliciesToRoles"; //removePoliciesToRoles
            public const string RemovePolicy = Base + "/policy/removePolicy";
            public const string RemoveRangePolicies = Base + "/policy/removeRangePolicies";
            public const string GetPoliciesActiveInRoleAsync = Base + "/policy/GetPoliciesActiveInRoleAsync";
            public const string GetPoliciesActiveInUserAsync = Base + "/policy/GetPoliciesActiveInUserAsync";
        }
        public static class Admin
        {
            public const string DeleteAccount = Base + "/admin/deleteAccount";
            public const string DeleteFile = Base + "/admin/deleteFile";
            public const string GetEmployeeInfo = Base + "/admin/getEmployeeInfo";
            public const string GetUsers = Base + "/admin/getUsers";
            public const string GetUsersDataGridPagination = Base + "/admin/GetUsersDataGridPagination";
            public const string LockAccount = Base + "/admin/lockAccount";
            public const string ResetPasswordAsync = Base + "/admin/resetPasswordAsync";
            public const string SendMail = Base + "/admin/sendMail";
            public const string SendMailAsync = Base + "/admin/sendMailAsync";
            public const string UploadFile = Base + "/admin/uploadFile";
            public const string GetUsers_SelectBox = Base + "/admin/getUsersSelectBox";
        }

        public static class Permission{
            public const string GetAllPermission = Base + "/permission/GetAllPermission";
            public const string SavePermission = Base + "/permission/SavePermission";
            public const string GetAllFunctionRole = Base + "/permission/GetAllFunctionRole";
            public const string CheckPermission = Base + "/permission/CheckPermission";
            public const string GetPermissionByRole = Base + "/permission/GetPermissionByRole";
        }
        public static class UserFactory
        {
            public const string GetAllFactoryByUserName = Base + "/UserFactory/GetAllFactoryByUserName";
            public const string AddUserFactory = Base + "/UserFactory/AddUserFactory";
        }
        public static class FunctionLog
        {
            public const string GetFunctionLogByUserName = Base + "/FunctionLog/GetFunctionLogByUserName";
            public const string AddFunctionLog = Base + "/FunctionLog/AddFunctionLog";
        }
    }
}