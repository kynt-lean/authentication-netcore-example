using System;
using Microsoft.AspNetCore.Authorization;

namespace BecamexIDC.Api.Domain
{
    public static class Roles
    {
        
        public const string AdminRole = "SI.AdminRole";
        public const string OperationRole = "SI.OperationRole";
         public const string LeaderRole = "SI.LeaderRole";
        public const string AnalysisRole = "SI.AnalysisRole";
        public const string StorekeeperRole = "SI.StorekeeperRole";
    }
    public static class BusinessRole
    {
        public const string Demo = Roles.AdminRole + "," + Roles.OperationRole;
    }
    public class RolesAttribute : AuthorizeAttribute
    {
        public RolesAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
    }
}