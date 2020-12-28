using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BecamexIDC.Authentication.Configuration;
using BecamexIDC.Authentication.Helpers;
using BecamexIDC.Authentication.Models;
using BecamexIDC.Authentication.Routes;
using BecamexIDC.Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace BecamexIDC.Authentication.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost(ApiRoutes.Role.AddToRole)]
        public async Task<IActionResult> AddToRole([FromBody] UserToRole userToRole)
        {
            return Ok(await _roleService.AddToRole(userToRole.Username, userToRole.roles));
        }
        [HttpDelete(ApiRoutes.Role.RemoveToRole)]
        public async Task<IActionResult> RemoveToRole(UserToRole userToRole)
        {
            return Ok(await _roleService.RemoveToRole(userToRole.Username, userToRole.roles));
        }
        [HttpGet(ApiRoutes.Role.GetRoles)]
        public IActionResult GetRoles()
        {
            return Ok(_roleService.GetRoles());
        }

        [HttpGet(ApiRoutes.Role.GetRolesDataGridPagination)]
        public async Task<IActionResult> GetRolesDataGridPagination(DataSourceLoadOptions loadOptions, string keyId)
        {
            return Ok(await _roleService.GetRolesDataGridPagination(loadOptions, keyId));
        }

        [HttpGet(ApiRoutes.Role.GetRolesByUser)]
        public async Task<IActionResult> GetRolesByUser(string userName)
        {
            return Ok(await _roleService.GetRolesByUser(userName));
        }   

        [HttpPost(ApiRoutes.Role.CreateRole)]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            return Ok(await _roleService.CreateRole(roleName));
        }
        [HttpGet(ApiRoutes.Role.GetUsersInRoleAsync)]
        public IActionResult GetUsersInRoleAsync(string roleName)
        {
            return Ok(_roleService.GetUsersInRoleAsync(roleName).Result);
        }

        [HttpGet(ApiRoutes.Role.GetRolesAsync)]
        public IActionResult GetRolesAsync(string userName)
        {
            return Ok(_roleService.GetRolesAsync(userName).Result);
        }
    }
}