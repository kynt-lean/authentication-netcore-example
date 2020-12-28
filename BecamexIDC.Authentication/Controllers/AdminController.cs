using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BecamexIDC.Authentication.Routes;
using BecamexIDC.Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Generic;
using NETCore.MailKit.Core;
using BecamexIDC.Authentication.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using BecamexIDC.Authentication.Configuration;
using BecamexIDC.Api.Domain;

namespace BecamexIDC.Authentication.Controllers
{

    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // [Roles(Roles.AdminRole)]
    public class AdminController : Controller
    {
        private readonly IIdentityService _dentityService;
        private readonly IRoleService _roleService;
        private readonly IPolicyService _policyService;
        private readonly IEmailService _emailService;

        public AdminController(IIdentityService identityService, IEmailService emailService,
                                IRoleService roleService,
                                IPolicyService policyService)
        {
            _dentityService = identityService;
            _emailService = emailService;
            _roleService = roleService;
            _policyService = policyService;
        }

        [HttpGet(ApiRoutes.Admin.GetUsers)]
        public IActionResult GetUsers()
        {
            return Ok(_dentityService.GetUsers());
        }


        [HttpGet, Route(ApiRoutes.Admin.GetUsers_SelectBox)]
        public IActionResult UI_SelectBox(DataSourceLoadOptions loadOptions, string keyId)
        {
            return Ok(_dentityService.GetUsers_SelectBox(loadOptions, keyId));
        }


        [HttpGet(ApiRoutes.Admin.GetUsersDataGridPagination)]
        public async Task<IActionResult> GetUsersDataGridPagination(DataSourceLoadOptions loadOptions, string keyId)
        {
            return Ok(await _dentityService.GetUsersDataGridPaginationAsync(loadOptions, keyId));
        }

        [HttpGet(ApiRoutes.Admin.GetEmployeeInfo)]
        public IActionResult GetEmployeeInfo()
        {
            return Ok(_dentityService.GetEmployeeInfo());
        }


        [HttpPut(ApiRoutes.Admin.LockAccount)]
        public async Task<IActionResult> LockAccount(string userName, bool isLock)
        {
            return Ok(await _dentityService.LockAccount(userName, isLock));
        }

        [HttpDelete(ApiRoutes.Admin.DeleteAccount)]
        public async Task<IActionResult> DeleteAccount(string userName)
        {
            return Ok(await _dentityService.DeleteAccount(userName));
        }


        [HttpPut(ApiRoutes.Admin.ResetPasswordAsync)]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPassword entity)
        {
            return Ok(await _dentityService.AdminResetPasswordAsync(entity));
        }

        [HttpPost(ApiRoutes.Admin.SendMailAsync)]
        public async Task<IActionResult> SendMailAsync(string mailTo, string subject, string message, bool isHtml = false)
        {
            await _emailService.SendAsync(mailTo, subject, message, isHtml);
            return Ok("Your's mail sent successed!");
        }
        [HttpPost(ApiRoutes.Admin.SendMail)]
        public IActionResult SendMail(string mailTo, string subject, string message, bool isHtml = false)
        {
            _emailService.Send(mailTo, subject, message, isHtml);
            return Ok("Your's mail sent successed!");
        }

        [HttpPost(ApiRoutes.Admin.UploadFile)]
        [AllowAnonymous]
        public async Task<IActionResult> UploadFileAsync(List<IFormFile> files, string PathFile)
        {
            long size = files.Sum(f => f.Length);
            string fileName = string.Empty;
            try
            {
                if (files.Count > 0)
                {
                    foreach (var formFile in files)
                    {
                        if (formFile.Length > 0)
                        {
                            fileName = formFile.FileName;
                            using (var stream = new FileStream(Path.Combine(PathFile, formFile.FileName), FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                    }
                }
                else
                    return Ok(new { success = false, count = files.Count, size, name = fileName, PathFile });
            }
            catch (System.Exception ex)
            {
                return Ok(new { sucess = false, message = ex.ToString() });
            }

            return Ok(new { success = true, count = files.Count, size, name = fileName, PathFile });
        }


        [HttpDelete(ApiRoutes.Admin.DeleteFile)]
        public JsonResult DeleteUploadFile(string fileName)
        {
            string fullPath = fileName;
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
    }
}