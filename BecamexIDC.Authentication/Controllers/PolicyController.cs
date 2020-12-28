using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BecamexIDC.Authentication.Models;
using BecamexIDC.Authentication.Routes;
using BecamexIDC.Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace BecamexIDC.Authentication.Controllers
{
    public class PolicyController : Controller
    {
        private readonly IPolicyService _policyService;
        public PolicyController(IPolicyService policyService)
        {
            _policyService = policyService;
        }
        [HttpPost(ApiRoutes.Policy.AddPolicy)]
        public async Task<IActionResult> AddPolicy(string Username, string Policy)
        {
            var operationResult = await _policyService.AddPolicy(Username, Policy);

            return Ok(operationResult);
        }
        [HttpPost(ApiRoutes.Policy.AddRangePolicies)]
        public async Task<IActionResult> AddRangePolicies(string Username, List<string> Policies)
        {
            var operationResult = await _policyService.AddRangePolicies(Username, Policies);

            return Ok(operationResult);
        }
        [HttpGet(ApiRoutes.Policy.GetPoliciesInUser)]
        public async Task<IActionResult> GetPolciesInUser(string userName)
        {
            return Ok(await _policyService.GetPoliciesInUser(userName));
        }
        [HttpGet(ApiRoutes.Policy.GetAllPolicies)]
        public async Task<IActionResult> GetAllPolicies(string type)
        {
            return Ok(await _policyService.GetAllPolicies(type));
        }
        [HttpDelete(ApiRoutes.Policy.RemovePolicy)]
        public async Task<IActionResult> RemovePolicy(string userName, string policy)
        {
            return Ok(await _policyService.RemovePolicy(userName, policy));
        }
        [HttpDelete(ApiRoutes.Policy.RemoveRangePolicies)]
        public async Task<IActionResult> RemoveRangePolicies(string userName, List<string> policies)
        {
            return Ok(await _policyService.RemoveRangePolicies(userName, policies));
        }

        //for roles
        [HttpPost(ApiRoutes.Policy.AddPoliciesToRoles)]
        public async Task<IActionResult> AddPoliciesToRole([FromBody] RolePolicyDto rolePolicy)
        {
            var operationResult = await _policyService.AddPoliciesToRoles(rolePolicy.roleName, rolePolicy.Policies);

            return Ok(operationResult);
        }

        [HttpPost(ApiRoutes.Policy.AddPoliciesToUser)]
        public async Task<IActionResult> AddPoliciesToUser([FromBody] UserPolicyDto userPolicy)
        {
            var operationResult = await _policyService.AddPoliciesToUser(userPolicy.userName, userPolicy.Policies);

            return Ok(operationResult);
        }

        [HttpDelete(ApiRoutes.Policy.RemovePoliciesToRoles)]
        public async Task<IActionResult> RemovePoliciesToRoles(string roleName, List<string> Policies)
        {
            var operationResult = await _policyService.RemovePoliciesToRoles(roleName, Policies);

            return Ok(operationResult);
        }

        [HttpGet(ApiRoutes.Policy.GetPoliciesInRoleAsync)]
        public async Task<IActionResult> GetPoliciesInRoleAsync(string roleName)
        {
            return Ok(await _policyService.GetPoliciesInRoleAsync(roleName));
        }

        [HttpGet(ApiRoutes.Policy.GetPoliciesActiveInRoleAsync)]
        public async Task<IActionResult> GetPoliciesActiveInRoleAsync(string roleName , string type)
        {
            return Ok(await _policyService.GetPoliciesActiveInRoleAsync(roleName,type));
        }

         [HttpGet(ApiRoutes.Policy.GetPoliciesActiveInUserAsync)]
        public async Task<IActionResult> GetPoliciesActiveInUserAsync(string userName , string type)
        {
            return Ok(await _policyService.GetPoliciesActiveInUserAsync(userName,type));
        }

    }
}