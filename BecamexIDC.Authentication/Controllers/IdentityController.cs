using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BecamexIDC.Authentication.Domain;
using BecamexIDC.Authentication.Helpers;
using BecamexIDC.Authentication.Routes;
using BecamexIDC.Authentication.Services;
using BecamexIDC.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BecamexIDC.Authentication.Models.Entities.BCMAppModels;

namespace BecamexIDC.Authentication.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IBCMAppService _BCMAppService;
        public IdentityController(IIdentityService identityService, IBCMAppService BCMAppService)
        {
            _identityService = identityService;
            _BCMAppService = BCMAppService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterAccount account)
        {
            var authResponse = await _identityService.RegisterAsync(account);

            if (!authResponse.Success)
            {
                return Ok(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
        #region old Login With LDAP && SystemDB
        [HttpPost(ApiRoutes.Identity.Login)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            var authResult  = new AuthenticationResult();
            request.Username = request.Username.ToLower();
            var authResponse = await _identityService.LoginAsync(request);
            if (!authResponse.Success)
            {
                var authSharePoint = await _identityService.LoginSharePoint(request);
                if (!authSharePoint.Success)
                {
                    var authLdapResponse = await _identityService.LDAPLoginAsync(request.Username, request.Password);
                    if (!authLdapResponse.Success)
                    {
                        return Ok(new AuthFailedResponse
                        {
                            Errors = authResponse.Errors == null ? (authSharePoint.Errors == null ? authLdapResponse.Errors : authSharePoint.Errors) : authResponse.Errors
                        });
                    }
                }
                else
                    authResult = authSharePoint;
            }
            else
                authResult = authResponse;        
            return Ok(authResult);
        }
        #endregion;

        [HttpPost(ApiRoutes.Identity.LoginSharePoint)]
        [AllowAnonymous]
        public async Task<IActionResult> LoginSharePoint([FromBody] Login request)
        {
            return Ok(await _identityService.LoginSharePoint(request));
        }

        [HttpPost(ApiRoutes.Identity.GetInfoSharePoint)]
        [AllowAnonymous]
        public async Task<IActionResult> GetInfoSharePoint([FromBody] Login request)
        {
            return Ok(await _identityService.GetSharePointLoginInfo(request));
        }

        [HttpPost(ApiRoutes.Identity.LDAPLogin)]
        public async Task<IActionResult> LDAPLogin([FromBody] Login request)
        {
            var authResponse = await _identityService.LDAPLoginAsync(request.Username, request.Password);

            if (!authResponse.Success)
            {
                return Ok(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.LDAP)]
        public IActionResult LDAP([FromBody] Login request)
        {
            return Ok(_identityService.LdapLogin(request.Username, request.Password));
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken,
                Cookie = authResponse.Cookie
            });
        }

        [HttpPost(ApiRoutes.Identity.ForgotPassword)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword entity)
        {
            string token = "";
            if (ModelState.IsValid)
            {
                token = await _identityService.ForgotPassword(entity.Email);
            }

            return Ok(new { Token = token });
        }

        [HttpPut(ApiRoutes.Identity.ResetPassword)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword entity)
        {
            if (!string.IsNullOrEmpty(entity.Token) && !string.IsNullOrEmpty(entity.Password))
                return Ok(await _identityService.UserResetPasswordAsync(entity));
            else
                return Ok(new { Erros = "Your token or password is not valid!" });

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost, Route(ApiRoutes.Identity.GetEOfficeUserInfo)]
        public async Task<IActionResult> GetEOfficeAccounts([FromBody] EOfficeUserQuery query)
        {
            try
            {
                var result = await _identityService.GetEOfficeUserInfo(query);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.ToString());
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost, Route(ApiRoutes.Identity.GetBCMAppUserInfo)]
        public async Task<IActionResult> GetBCMAppUserInfo([FromBody] BCMAppUserQuery query)
        {
            try
            {
                var result = await _BCMAppService.GetBCMAppUserInfo(query);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.ToString());
            }
        }

        [HttpGet, Route(ApiRoutes.Identity.GetEofficeUserNameOnly)]
        public async Task<IActionResult> GetEofficeUserNameOnly()
        {
            try
            {
                var result = await _identityService.GetEofficeUserNameOnly();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.ToString());
            }
        }

        [HttpGet, Route(ApiRoutes.Identity.GetEOfficeAccountsSharePoint)]
        public async Task<IActionResult> GetEOfficeAccountsSharePoint()
        {
            try
            {
                var result = await _identityService.GetEOfficeAccountsSharePoint();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.ToString());
            }
        }
    }
}