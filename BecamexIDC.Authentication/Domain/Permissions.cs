using Microsoft.AspNetCore.Authorization;

namespace BecamexIDC.Authentication.Domain
{
    public static class RolePermissions
    {
        public static class Dashboards
        {
            public const string View = "Permissions.Dashboards.View";
            public const string Create = "Permissions.Dashboards.Create";
            public const string Edit = "Permissions.Dashboards.Edit";
            public const string Delete = "Permissions.Dashboards.Delete";
        }

        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
        }
    }
    public static class UserPermission
    {
        public const string View = "ViewPoilcy";
        public const string Create = "CreatePoilcy";
    }
    public class CustomClaimTypes
    {
        public const string Permission = "permission";
    }
    
}