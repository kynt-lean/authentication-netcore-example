using BecamexIDC.Authentication.Models.ViewModels;
using BecamexIDC.Authentication.Routes;
using BecamexIDC.Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BecamexIDC.Authentication.Controllers
{
    public class UserFactoryController : Controller
    {
        private readonly IUserFactoryService _userFactoryService;

        public UserFactoryController(IUserFactoryService userFactoryService)
        {
            _userFactoryService = userFactoryService;
        }

        [HttpGet(ApiRoutes.UserFactory.GetAllFactoryByUserName)]
        public async Task<IActionResult> GetAllFactoryByUserName(string userName)
        {
            return Ok(await _userFactoryService.GetFactoryByUser(userName));
        }

        [HttpPost(ApiRoutes.UserFactory.AddUserFactory)]
        public async Task<IActionResult> Add(string userName, [FromBody] List<UserFactoryViewModel> model)
        {
                return Ok(await _userFactoryService.Add(userName, model));
        }
    }
}
