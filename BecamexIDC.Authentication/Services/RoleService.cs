using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BecamexIDC.Authentication.Configuration;
using BecamexIDC.Authentication.Data;
using BecamexIDC.Authentication.Domain;
using BecamexIDC.Authentication.Helpers;
using BecamexIDC.Authentication.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Identity;

namespace BecamexIDC.Authentication.Services
{
    public interface IRoleService
    {
      
        Task<OperationResult> CreateRole(string roleName);
        Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName);
        Task<IList<string>> GetRolesAsync(string userName);       
        Task<OperationResult> AddToRole(string userName, IEnumerable<string> roleNames);
        Task<OperationResult> RemoveToRole(string userName, IEnumerable<string> roleNames);
        IQueryable<IdentityRole> GetRoles();
        Task<LoadResult> GetRolesDataGridPagination(DataSourceLoadOptions loadOptions, string keyId);

        Task<List<RoleUserViewModel>> GetRolesByUser(string userName);
    }
    public class RoleService : IRoleService
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthDbContext _context;

        public RoleService(UserManager<IdentityUser> userManager,
                                SignInManager<IdentityUser> singInManger,
                                AuthDbContext context,
                                RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = singInManger;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<OperationResult> CreateRole(string roleName)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = roleName
            };
            IdentityResult result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                return new OperationResult
                {
                    Success = true,
                    Caption = "Added Success",
                    Message = string.Format("Add Role is complete")

                };
            }
            else
            {
                return new OperationResult
                {
                    Success = false,
                    Caption = "Failed",
                    Message = result.Errors.ToString()

                };
            }
        }

        public async Task<OperationResult> AddToRole(string userName, IEnumerable<string> roleNames)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
            if (identityUser != null)
            {
                //remove all role user
                var roles = await _userManager.GetRolesAsync(identityUser);
                await _userManager.RemoveFromRolesAsync(identityUser, roles);

                var addRole = await _userManager.AddToRolesAsync(identityUser, roleNames);
                if (!addRole.Succeeded)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Add role failed " + addRole.Errors.ToString(),
                        Caption = "Error"
                    };
                }
            }
            return new OperationResult
            {
                Success = true,
                Message = "Add role Complete ",
                Caption = "Success"
            };
        }
        public async Task<OperationResult> RemoveToRole(string userName, IEnumerable<string> roleNames)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
            if (identityUser != null)
            {
                var addRole = await _userManager.RemoveFromRolesAsync(identityUser, roleNames);
                if (!addRole.Succeeded)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Add role failed " + addRole.Errors.ToString(),
                        Caption = "Error"
                    };
                }
            }
            return new OperationResult
            {
                Success = true,
                Message = "Add role Complete ",
                Caption = "Success"
            };
        }

        public IQueryable<IdentityRole> GetRoles()
        {
            return _roleManager.Roles;
        }

        public async Task<LoadResult> GetRolesDataGridPagination(DataSourceLoadOptions loadOptions, string keyId)
        {
            loadOptions.PrimaryKey = new[] { keyId };
            return await Task.FromResult(DataSourceLoader.Load(_roleManager.Roles, loadOptions));
        }

        public async Task<List<RoleUserViewModel>> GetRolesByUser(string userName)
        {
            var roles = _roleManager.Roles;
            var userRole = await GetRolesAsync(userName);

            var listResult = new List<RoleUserViewModel>();
            foreach (var item in roles)
            {
                var roleUser = new RoleUserViewModel();
                roleUser.RoleId = item.Id;
                roleUser.RoleName = item.Name;
                var role = userRole.FirstOrDefault(z => z == item.Name);
                roleUser.Active = role != null ? true : false;
                listResult.Add(roleUser);
            }
            return listResult;
        }

        public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }

        public async Task<IList<string>> GetRolesAsync(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            return await _userManager.GetRolesAsync(currentUser);
        }        

    }
}