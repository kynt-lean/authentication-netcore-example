using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BecamexIDC.Authentication.Data;
using BecamexIDC.Authentication.Domain;
using BecamexIDC.Authentication.Helpers;
using BecamexIDC.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BecamexIDC.Authentication.Services
{
    public interface IPolicyService
    {
        Task<OperationResult> AddPolicy(string userName, string policy);
        Task<OperationResult> AddRangePolicies(string userName, List<string> policies);
        Task<OperationResult> RemovePolicy(string userName, string policy);
        Task<OperationResult> RemoveRangePolicies(string userName, List<string> policies);
        Task<IList<Claim>> GetPoliciesInUser(string userName);
        Task<IQueryable<Policies>> GetAllPolicies(string type);

        //for Roles
        Task<OperationResult> AddPoliciesToRoles(string roleName, List<string> policies);
         Task<OperationResult> AddPoliciesToUser(string userName, List<string> policies);
        Task<OperationResult> RemovePoliciesToRoles(string roleName, List<string> policies);
        Task<IList<Claim>> GetPoliciesInRoleAsync(string roleName);
        Task<List<PoliciesActiveRoleViewModel>> GetPoliciesActiveInRoleAsync(string roleName,string type);
        Task<List<Policies>> GetPolicies(string type="");
        Task<List<PoliciesActiveUserPolicyViewModel>> GetPoliciesActiveInUserAsync(string userName,string type);
    }
    public class PolicyService : IPolicyService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthDbContext _context;
        private OperationResult operationResult = new OperationResult();

        public PolicyService(UserManager<IdentityUser> userManager,
                                SignInManager<IdentityUser> singInManger,
                                 AuthDbContext context,
                                RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = singInManger;
            _roleManager = roleManager;
            _context = context;
        }
        private async Task<OperationResult> PolicyToUser(string userName, string policy, string action)
        {
            var user = await _userManager.FindByNameAsync(userName);
            IdentityResult result = new IdentityResult();
            if (action == "Add")
                result = await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permission, policy));
            else
                result = await _userManager.RemoveClaimAsync(user, new Claim(CustomClaimTypes.Permission, policy));
            if (result.Succeeded)
                operationResult = new OperationResult("Success", action + " Policies to user successed!", true);
            else
                operationResult = new OperationResult("Error", action + " Policies to user failed!", false);
            return operationResult;
        }
        public async Task<IList<Claim>> GetPoliciesInUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return await _userManager.GetClaimsAsync(user);
        }
        public Task<IQueryable<Policies>> GetAllPolicies(string type)
        {
            return Task.FromResult(_context.Policies.Where(x => x.Type == type));
        }

        public Task<OperationResult> AddPolicy(string userName, string policy)
        {
            return PolicyToUser(userName, policy, "Add");
        }

        public async Task<OperationResult> AddRangePolicies(string userName, List<string> policies)
        {
            foreach (var policy in policies)
            {
                operationResult = await PolicyToUser(userName, policy, "Add");
            }
            return operationResult;
        }

        public async Task<OperationResult> RemovePolicy(string userName, string policy)
        {
            return await PolicyToUser(userName, policy, "Remove");
        }

        public async Task<OperationResult> RemoveRangePolicies(string userName, List<string> policies)
        {
            foreach (var policy in policies)
            {
                operationResult = await PolicyToUser(userName, policy, "Remove");
            }
            return operationResult;
        }

        public async Task<OperationResult> AddPoliciesToRoles(string roleName, List<string> policies)
        {
            var currentRole = _roleManager.Roles.FirstOrDefault(role => role.Name == roleName);
            var  currentClams =await _roleManager.GetClaimsAsync(currentRole);
            foreach (var claim in currentClams)
            {
                await _roleManager.RemoveClaimAsync(currentRole,claim);
            }
            
            return await PoliciesToRoles(roleName, policies, "Add");
        }

        public async Task<OperationResult> AddPoliciesToUser(string userName, List<string> policies)
        {
            var currentUser =await _userManager.FindByNameAsync(userName);
            var  currentClams =await _userManager.GetClaimsAsync(currentUser);
            foreach (var claim in currentClams)
            {
                await _userManager.RemoveClaimAsync(currentUser,claim);
            }
            
            return await PoliciesToUser(userName, policies, "Add");
        }

        public async Task<OperationResult> RemovePoliciesToRoles(string roleName, List<string> policies)
        {
            return await PoliciesToRoles(roleName, policies, "Remove");
        }
        private async Task<OperationResult> PoliciesToRoles(string roleName, List<string> policies, string actions)
        {
            var roles = await _roleManager.FindByNameAsync(roleName);
            IdentityResult result = new IdentityResult();
            if (roles != null)
            {
                if(policies.Count==0){
                    return new OperationResult
                    {
                        Success = true,
                        Message = actions + " Policies add success!",
                        Caption = "Success"
                    };
                }
                foreach (var policy in policies)
                {
                    if (actions == "Add")
                        result = await _roleManager.AddClaimAsync(roles, new Claim(CustomClaimTypes.Permission, policy));
                    else
                        result = await _roleManager.RemoveClaimAsync(roles, new Claim(CustomClaimTypes.Permission, policy));
                }
                if (!result.Succeeded)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = actions + " Policies to role failed!",
                        Caption = "Error"
                    };
                }
            }
            return new OperationResult
            {
                Success = true,
                Message = actions + " Policies to role successed! ",
                Caption = "Success"
            };
        }

         private async Task<OperationResult> PoliciesToUser(string userName, List<string> policies, string actions)
        {
            var user = await _userManager.FindByNameAsync(userName);
            IdentityResult result = new IdentityResult();
            if (user != null)
            {
                if(policies.Count==0){
                    return new OperationResult
                    {
                        Success = true,
                        Message = actions + " Policies add success!",
                        Caption = "Success"
                    };
                }
                foreach (var policy in policies)
                {
                    if (actions == "Add")
                        result = await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permission, policy));
                    else
                        result = await _userManager.RemoveClaimAsync(user, new Claim(CustomClaimTypes.Permission, policy));
                }
                if (!result.Succeeded)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = actions + " Policies to role failed!",
                        Caption = "Error"
                    };
                }
            }
            return new OperationResult
            {
                Success = true,
                Message = actions + " Policies to role successed! ",
                Caption = "Success"
            };
        }


        public Task<IList<Claim>> GetPoliciesInRoleAsync(string roleName)
        {
            var currentRole = _roleManager.Roles.FirstOrDefault(role => role.Name == roleName);
            return _roleManager.GetClaimsAsync(currentRole);
        }

        public async  Task<List<Policies>> GetPolicies(string type="")
        {
            var query = _context.Policies.AsQueryable();
            if(!string.IsNullOrEmpty(type))
               query =  query.Where(x=>x.Type ==type);
            return await query.ToListAsync();
        }

        public async Task<List<PoliciesActiveRoleViewModel>> GetPoliciesActiveInRoleAsync(string roleName,string type)
        {
            var policies =await GetPolicies(type);
            var currentRole = _roleManager.Roles.FirstOrDefault(role => role.Name == roleName);
            var clams =await _roleManager.GetClaimsAsync(currentRole);
            var listRoleClams = new List<PoliciesActiveRoleViewModel>();
            foreach (var item in policies)
            {
                var policyActive =new PoliciesActiveRoleViewModel();
                policyActive.PolicyName = item.Policy;

                var clam =clams.FirstOrDefault(x=>x.Value== item.Policy);
                if(clam!= null)  {
                     policyActive.Active = clam!=null?true:false;
                }  
                listRoleClams.Add(policyActive);
            }
            
            return listRoleClams;
        }

        public async Task<List<PoliciesActiveUserPolicyViewModel>> GetPoliciesActiveInUserAsync(string userName,string type)
        {
            var policies =await GetPolicies(type);
            var currentUser =await _userManager.FindByNameAsync(userName);
            
            var clams =await _userManager.GetClaimsAsync(currentUser);
            var listRoleClams = new List<PoliciesActiveUserPolicyViewModel>();
            foreach (var item in policies)
            {
                var policyActive =new PoliciesActiveUserPolicyViewModel();
                policyActive.PolicyName = item.Policy;

                var clam =clams.FirstOrDefault(x=>x.Value== item.Policy);
                if(clam!= null)  {
                     policyActive.Active = clam!=null?true:false;
                }  
                listRoleClams.Add(policyActive);
            }
            
            return listRoleClams;
        }


    }
}