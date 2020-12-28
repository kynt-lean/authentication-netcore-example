using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BecamexIDC.Authentication.Configuration;
using BecamexIDC.Authentication.Helpers;
using BecamexIDC.Authentication.Models;
using BecamexIDC.Authentication.Models.Dtos;
using BecamexIDC.Authentication.Routes;
using BecamexIDC.Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace BecamexIDC.Authentication.Controllers
{
    public class PermissionController : Controller
    {
        private readonly IPermissionService _permissionService;
        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet(ApiRoutes.Permission.GetAllPermission)]
        public async Task<IActionResult> GetAllPermission()
        {
            return Ok(await _permissionService.GetAll());
        }

        [HttpPost(ApiRoutes.Permission.SavePermission)]
        public async Task<IActionResult> SavePermission([FromBody]PermissionDto model) 
        {
            return Ok(await _permissionService.SavePermission(model.listPermmission, model.roleId));
        }

        [HttpGet(ApiRoutes.Permission.GetAllFunctionRole)]
        public async Task<IActionResult> GetAllFunctionRole(string roleId) 
        {
            return Ok(await _permissionService.GetAllFunctionRole(roleId));
        }

        
        [HttpPost(ApiRoutes.Permission.CheckPermission)]
        public async Task<IActionResult> CheckPermission([FromBody]CheckPermissionDto model) 
        {
            return Ok(await _permissionService.CheckPermission(model.functionCode,model.action,model.roles));
        }
        
        [HttpPost(ApiRoutes.Permission.GetPermissionByRole)]
        public async Task<IActionResult> GetPermissionByRole([FromBody]string[] roles) 
        {
            return Ok(await _permissionService.GetPermissionByRole(roles));
        }
        
    }
}