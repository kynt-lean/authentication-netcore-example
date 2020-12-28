using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BecamexIDC.Authentication.Filters;
using BecamexIDC.Authentication.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NETCore.MailKit.Core;
using BecamexIDC.Authentication.Options;
using Microsoft.AspNetCore.Http;
using BecamexIDC.Authentication.Helpers;

namespace BecamexIDC.Authentication.Controllers
{
    // [Route("api/[controller]")]
    // [ApiController]
    [ApiKeyAuth]
    
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // [Authorize(Roles = "Admin")]
    public class SecreteController : ControllerBase
    {
        private readonly IEmailService _EmailService;
        public SecreteController(IEmailService EmailService)
        {
            _EmailService = EmailService;
        }
        [HttpGet, Route("Index")]
        public string Index()
        {
            return "Hello World";
        }

        [HttpGet, Route("Sendmail")]
        public IActionResult Sendmail(string mailTo, string subject, string message, bool isHtml = false)
        {
            try
            {

                _EmailService.Send(mailTo, subject, message, isHtml);
            }
            catch (System.Exception ex)
            {

                return Ok("Error: " + ex.ToString());
            }

            return Ok("Your's mail sent successed!");
        }
        //set permission for Roles with policy
        [HttpGet, Route("GroupView")]
        [Authorize(RolePermissions.Dashboards.View)]
        public ActionResult GroupView()
        {
            return Ok("View");
        }
        //set permission for User with policy
        // với user dung.lt chưa được cấp quyền
        [HttpGet, Route("View")]
        [Authorize(UserPermission.View)]
        public ActionResult View()
        {
            return Ok("View");
        }

        //set permission for User with policy
        [HttpGet, Route("Create")]
        //[Authorize(UserPermission.Create)]
        [AllowAnonymous]
        public ActionResult Create()
        {
            var currentUser = HttpContext.GetUserId();
            return Ok("Current User" + currentUser);
        }
    }

}