using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BecamexIDC.Authentication.Data;
using BecamexIDC.Authentication.Options;
using BecamexIDC.Authentication.Domain;
using BecamexIDC.Authentication.Helpers;
using Novell.Directory.Ldap;
using BecamexIDC.Authentication.Models;
using System.Data;
using BecamexIDC.DataExtension;
using BecamexIDC.Authentication.Configuration;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Net;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using static BecamexIDC.Authentication.Services.IdentityService;

namespace BecamexIDC.Authentication.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(RegisterAccount registerAccount);
        Task<AuthenticationResult> LoginAsync(Login userLogin);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken,string password);
     

        IQueryable<IdentityUser> GetUsers();
        LoadResult GetUsers_SelectBox(DataSourceLoadOptions loadOptions, string key);
        Task<LoadResult> GetUsersDataGridPaginationAsync(DataSourceLoadOptions loadOptions, string keyId);

        //Users
        Task<IdentityResult> LockAccount(string userName, bool isLock);
        Task<IdentityResult> DeleteAccount(string userName);
        Task<OperationResult> AdminResetPasswordAsync(ResetPassword entity);
        Task<OperationResult> UserResetPasswordAsync(ResetPassword entity);
        Task<string> ForgotPassword(string email);

       
        Task<DataTable> GetSharePointLoginInfo(Login userLogin);
        DataTable GetEmployeeInfo();
        //Login Flow
        Task<AuthenticationResult> LDAPLoginAsync(string userName, string password);
        AuthenticateResult LdapLogin(string userName, string Password);
        Task<AuthenticationResult> LoginSharePoint(Login userLogin);



        //Users Info
        Task<List<EOfficeUserInfo>> GetEOfficeAccountsSharePoint();
        Task<List<EOfficeUserInfo>> GetEOfficeUserInfo(EOfficeUserQuery query);
        Task<List<EOfficeUserNameOnly>> GetEofficeUserNameOnly();
        Task<string> GetEofficeUserNameById(string Id);

    }
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly LDAPSettings _ldapSettings;
        private readonly WcfSettings _wcfSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IQueryExtension _queryExtension;
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;

        public IdentityService(UserManager<IdentityUser> userManager,
                                JwtSettings jwtSettings,
                                TokenValidationParameters tokenValidationParameters,
                                AuthDbContext context,
                                LDAPSettings ldapSettings,
                                SignInManager<IdentityUser> singInManger,
                                IQueryExtension queryExtension,
                                WcfSettings wcfSettings,
                                RoleManager<IdentityRole> roleManager,
                                IConfiguration configuration)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _signInManager = singInManger;
            _context = context;
            _roleManager = roleManager;
            _ldapSettings = ldapSettings;
            _wcfSettings = wcfSettings;
            _queryExtension = queryExtension;
            _configuration = configuration;
        }

        public async Task<AuthenticationResult> RegisterAsync(RegisterAccount registerAccount)
        {
            var existingUser = await _userManager.FindByNameAsync(registerAccount.Username);
            if (existingUser != null) //???
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this userName  already exists" }
                };
            }
            var newUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = registerAccount.Email,
                UserName = registerAccount.Username,
                PhoneNumber = registerAccount.PhoneNumber
            };

            var createdUser = await _userManager.CreateAsync(newUser, registerAccount.Password);
            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResultForUserAsync(newUser, registerAccount.Password);
        }

        public IQueryable<IdentityUser> GetUsers()
        {
            return _userManager.Users;
        }

        public LoadResult GetUsers_SelectBox(DataSourceLoadOptions loadOptions, string key)
        {
            //_logger.LogInformation("SelectBox SmartInTopics {Time}:", DateTime.UtcNow.ToString());
            loadOptions.PaginateViaPrimaryKey = true;
            try
            {
                loadOptions.PrimaryKey = new[] { "Id" };
                return DataSourceLoader.Load(_userManager.Users, loadOptions);
            }
            catch (System.Exception ex)
            {
                //_logger.LogError("Error when SelectBox SmartInTopics {Time}:" + ex.ToString(), DateTime.UtcNow.ToString());
                throw new ArgumentNullException(ex.ToString());
            }
        }

        public async Task<AuthenticationResult> LoginAsync(Login userLogin)
        {
            var user = await _userManager.FindByNameAsync(userLogin.Username);//.FindByEmailAsync(userName);
            AuthenticationResult authenResult = new AuthenticationResult();
            if (user != null)
                authenResult.Errors = new[] { "User does not exist" };
            if (user != null && !(
                await _userManager.CheckPasswordAsync(user, userLogin.Password)))
                authenResult.Errors = new[] { "User/password combination is wrong" };
            var result = await _signInManager.PasswordSignInAsync(userLogin.Username, userLogin.Password, userLogin.RememberMe, true);
            if (result.Succeeded)
            {
                // after 5 minutes can login again 
                // if (await _userManager.IsLockedOutAsync(user))
                //     await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                // else
                //check if login successed!
                return await GenerateAuthenticationResultForUserAsync(user, userLogin.Password);
            }
            if (result.IsLockedOut)
                authenResult.Errors = new[] { "Your account have been locked!\n Please contact with Administrator" };
            return authenResult;

        }
        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken,string password)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Invalid Token" } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user, password);
        }
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }
        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user, string password)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id),
                new Claim("usr", user.UserName)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var userFactory = _context.UserFactory.Where(x => x.UserId == user.Id).ToList();
            foreach (var factory in userFactory)
            {
                claims.Add(new Claim("factories", factory.FactoryId.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                //Expires = DateTime.UtcNow.Add(TimeSpan.FromSeconds(15)),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            var eOfficeCookie = new Cookie();
            if (!string.IsNullOrEmpty(password))
            {
                try
                {
                    eOfficeCookie = await GeteOfficeAuthCookie(Environment.GetEnvironmentVariable("BCM_WCF_ADDRESS"), user.UserName, password);
                }
                catch (Exception)
                {
                }                
            }
            if (eOfficeCookie != null)
            {
                var expiresCus = DateTime.Now.AddDays(4);
                expiresCus = Truncate(expiresCus, TimeSpan.FromSeconds(1));
                return new AuthenticationResult
                {
                    Success = true,
                    Token = tokenHandler.WriteToken(token),
                    RefreshToken = refreshToken.Token,
                    Cookie = new eOfficeCookie
                    {
                        Expires = eOfficeCookie.Expires < DateTime.Now ? expiresCus : eOfficeCookie.Expires,
                        Name = eOfficeCookie.Name,
                        Value = eOfficeCookie.Name == "" ? "" : eOfficeCookie.Name + "=" + eOfficeCookie.Value,
                        Value_Cookie = eOfficeCookie.Value,
                        Domain = eOfficeCookie.Domain
                    }
                };
            }
            else
            {
                return new AuthenticationResult
                {
                    Success = false
                };
            }           
        }
        private DateTime Truncate(DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero) return dateTime; // Or could throw an ArgumentException
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) return dateTime; // do not modify "guard" values
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
        public async Task<AuthenticationResult> LDAPLoginAsync(string userName, string password)
        {
            if (LoginLdapLinux(userName, password))
            {
                var user = await _userManager.FindByNameAsync(userName);//.FindByEmailAsync(userName);
                //get user email
                var userquery = new EOfficeUserQuery();
                userquery.Account = userName;
                var usersinfo = await GetEOfficeUserInfo(userquery);
                var userinfo = usersinfo.FirstOrDefault();
                if (userinfo != null)
                {
                    if (userinfo.Email == null)
                    {
                        userinfo.Email = userName + "@becamex.com.vn";
                    }
                }                
                //end get user email
                var registerAccount = new RegisterAccount()
                {
                    Username = userName,
                    Email = userinfo != null ? userinfo.Email : userName + "@becamex.com.vn",
                    Password = password
                };
                if (user == null)
                    return await RegisterAsync(registerAccount);
                else
                    return await GenerateAuthenticationResultForUserAsync(user, password);
            }
            else
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "User does not exist" }
                };
            }
        }
        private async Task<AuthenticationResult> LoginLdapWindow(string userName, string password)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _ldapSettings.Address + ":" + _ldapSettings.Port, _ldapSettings.Domain + "\\" + _ldapSettings.Account, _ldapSettings.Password))
            {
                if (context.ValidateCredentials(userName, password))
                {
                    using (var de = new DirectoryEntry(_ldapSettings.Address + ":" + _ldapSettings.Port))
                    using (var ds = new DirectorySearcher(de))
                    {

                        var identities = new ClaimsIdentity("custom auth type");
                        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), "");

                        var user = await _userManager.FindByNameAsync(userName);//.FindByEmailAsync(userName);
                        //get user email
                        var userquery = new EOfficeUserQuery();
                        userquery.Account = userName;
                        var usersinfo = await GetEOfficeUserInfo(userquery);
                        var userinfo = usersinfo.FirstOrDefault();
                        if (userinfo.Email == null)
                        {
                            userinfo.Email = userName + "@becamex.com.vn";
                        }
                        //end get user email
                        var registerAccount = new RegisterAccount()
                        {
                            Username = userName,
                            Email = userinfo.Email,
                            Password = password
                        };

                        if (user == null)
                            return await RegisterAsync(registerAccount);
                        else
                            return await GenerateAuthenticationResultForUserAsync(user, password);
                    }
                }
                else
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User does not exist" }
                    };
                }
            }
        }
        private bool LoginLdapLinux(string userName, string password)
        {
            string userDn = $"{_ldapSettings.Domain}\\{userName}";
            bool action = true;
            using (var connection = new LdapConnection { SecureSocketLayer = false })
            {
                try
                {
                    connection.Connect(_ldapSettings.Address, Int32.Parse(_ldapSettings.Port));
                    connection.Bind(userDn, password);
                    if (connection.Bound) action = true;
                }
                catch (System.Exception)
                {
                    action = false;
                }
            }
            return action;
        }
        public AuthenticateResult LdapLogin(string userName, string Password)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _ldapSettings.Address + ":" + _ldapSettings.Port, _ldapSettings.Domain + "\\" + _ldapSettings.Account, _ldapSettings.Password))
            {
                if (context.ValidateCredentials(userName, Password))
                {
                    using (var de = new DirectoryEntry(_ldapSettings.Address + ":" + _ldapSettings.Port))
                    using (var ds = new DirectorySearcher(de))
                    {
                        var identities = new ClaimsIdentity("custom auth type");
                        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), "");
                        return AuthenticateResult.Success(ticket);
                    }
                }
            }
            return AuthenticateResult.Fail("Invalid user");
        }
        //Admin reset Password
        public async Task<OperationResult> AdminResetPasswordAsync(ResetPassword entity)
        {
            var existingUser = await _userManager.FindByNameAsync(entity.UserName);
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            var data = await _userManager.ResetPasswordAsync(existingUser, resetToken, entity.Password);
            if (data.Succeeded)
            {
                return new OperationResult
                {
                    Success = true,
                    Caption = "Sucess",
                    Message = string.Format("{0}'s password have been reset", existingUser.UserName)
                };
            }
            else
            {
                return new OperationResult
                {
                    Success = false,
                    Caption = "Error",
                    Message = string.Format("{0}'s password can not been reset", entity.UserName)
                };
            }
        }
        public async Task<LoadResult> GetUsersDataGridPaginationAsync(DataSourceLoadOptions loadOptions, string keyId)
        {
            loadOptions.PrimaryKey = new[] { keyId };
            return await Task.FromResult(DataSourceLoader.Load(_userManager.Users, loadOptions));
        }
        public async Task<IdentityResult> LockAccount(string userName, bool isLock)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            if (isLock)
                return await _userManager.SetLockoutEndDateAsync(currentUser, DateTime.Today.AddYears(100));
            else
                return await _userManager.SetLockoutEndDateAsync(currentUser, null);

        }
        public async Task<IdentityResult> DeleteAccount(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            var currentToken = _context.RefreshTokens.Where(x => x.UserId == currentUser.Id);
            if (currentToken.Count() > 0)
                _context.RefreshTokens.RemoveRange(currentToken);
            return await _userManager.DeleteAsync(currentUser);
        }
        public async Task<string> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                return token;
            }
            return "";
        }
        public async Task<OperationResult> UserResetPasswordAsync(ResetPassword entity)
        {
            var user = await _userManager.FindByEmailAsync(entity.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, entity.Token, entity.Password);
                if (!result.Succeeded)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Your token or password invalid\n Please try again!",
                        Caption = "Error"
                    };
                }
            }
            return new OperationResult
            {
                Success = true,
                Message = " Your password have been reset!",
                Caption = "Successed"
            };

        }
        private async Task<OperationResult> canLogin(Login userLogin)
        {
            var operationResult = new OperationResult();
            operationResult.Success = false; //set default value
            using (var authentication = new SharePointApi.AuthenticationSoapClient(SharePointApi.AuthenticationSoapClient.EndpointConfiguration.AuthenticationSoap, _wcfSettings.RemoteAddress))
            {

                var loginRespone = await authentication.LoginAsync(userLogin.Username, userLogin.Password);
                if (!string.IsNullOrEmpty(loginRespone.Body.LoginResult.CookieName))
                {
                    operationResult.Success = true;
                    operationResult.Message = "Login successed!";
                    operationResult.Caption = "Successed!";
                }
                else
                {
                    operationResult.Success = false;
                    operationResult.Message = loginRespone.Body.LoginResult.ErrorCode.ToString();
                    operationResult.Caption = "Failed!";
                }
            }
            return operationResult;
        }
        public async Task<DataTable> GetSharePointLoginInfo(Login userLogin)
        {
            var result = new DataTable();
            var data = GetEmployeeInfo();
            var filter = data.Select("AccountName = '" + userLogin.Username + "'");
            if (filter.Count() > 0)
                result = filter.CopyToDataTable();
            return await Task.FromResult(result);
        }
        public DataTable GetEmployeeInfo()
        {
            string query = @"select * from [UAT_DATA_EOFFICE_BG_Becamex].[dbo].[PersonalProfile]";

            return _queryExtension.ExecuteQuery(query, new object[] { }).Tables[0];
        }
        public async Task<AuthenticationResult> LoginSharePoint(Login userLogin)
        {
            var canlogin = await canLogin(userLogin);
            if (canlogin.Success)
            {
                var user = await _userManager.FindByNameAsync(userLogin.Username);//.FindByEmailAsync(userName);
                //get user email
                var userquery = new EOfficeUserQuery();
                userquery.Account = userLogin.Username;
                var usersinfo = await GetEOfficeUserInfo(userquery);
                var userinfo = usersinfo.FirstOrDefault();
                if (userinfo != null)
                {
                    if (userinfo.Email == null)
                    {
                        userinfo.Email = userLogin.Username + "@becamex.com.vn";
                    }
                }

                //end get user email
                var registerAccount = new RegisterAccount()
                {
                    Username = userLogin.Username,
                    Email = userinfo != null ? userinfo.Email : userLogin.Username + "@becamex.com.vn",
                    Password = userLogin.Password

                };
                if (user == null)
                    return await RegisterAsync(registerAccount);
                else
                    return await GenerateAuthenticationResultForUserAsync(user, userLogin.Password);
            }
            else
            {
                return new AuthenticationResult
                {
                    Success = canlogin.Success,
                    Errors = new[] { canlogin.Message }
                };
            }
        }
        public async Task<Cookie> GeteOfficeAuthCookie(String url, String uname, String pswd)
        {
            string envelope =
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">"
                        + "<soap:Body>"
                        + "<Login xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">"
                        + "<username>{0}</username>"
                        + "<password>{1}</password>"
                        + "</Login>" + "</soap:Body>"
                        + "</soap:Envelope>";

            CookieContainer cookieJar = new CookieContainer();
            Uri authServiceUri = new Uri(url + Environment.GetEnvironmentVariable("BCM_WCF_AUTHENTICATION"));
            HttpWebRequest spAuthReq = HttpWebRequest.Create(authServiceUri) as HttpWebRequest;
            spAuthReq.CookieContainer = cookieJar;
            spAuthReq.Headers["SOAPAction"] = "http://schemas.microsoft.com/sharepoint/soap/Login";
            spAuthReq.ContentType = "text/xml; charset=utf-8";
            spAuthReq.Method = "POST";
            envelope = string.Format(envelope, WebUtility.HtmlEncode(uname), WebUtility.HtmlEncode(pswd));
            StreamWriter streamWriter = new StreamWriter(spAuthReq.GetRequestStream());
            streamWriter.Write(envelope);
            streamWriter.Close();
            HttpWebResponse response = spAuthReq.GetResponse() as HttpWebResponse;
            if (response != null && response.Cookies.Count > 0)
            {
                Cookie returnValue = response.Cookies[0];
                response.Close();
                return await Task.FromResult(returnValue);
            }
            else
                return null;
        }
        // UserInfo
        public async Task<List<EOfficeUserInfo>> GetEOfficeUserInfo(EOfficeUserQuery query)
        {
            var listquery = await _context.EOfficeUserInfo.ToListAsync();
            if (!string.IsNullOrEmpty(query.FullName))
            {
                listquery = listquery.Where(x => x.Title.ToUpper().Contains(query.FullName.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(query.StaffId))
            {
                listquery = listquery.Where(x => x.StaffId.ToUpper().Contains(query.StaffId.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(query.Position))
            {
                listquery = listquery.Where(x => x.Position.ToUpper().Contains(query.Position.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(query.Address))
            {
                listquery = listquery.Where(x => x.Address.ToUpper().Contains(query.Address.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(query.Mobile))
            {
                listquery = listquery.Where(x => x.Mobile.ToUpper().Contains(query.Mobile.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(query.Email))
            {
                listquery = listquery.Where(x => x.Email.ToUpper().Contains(query.Email.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(query.PersonId))
            {
                listquery = listquery.Where(x => x.PersonId.ToUpper().Contains(query.PersonId.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(query.Department))
            {
                listquery = listquery.Where(x => x.Department.ToUpper().Contains(query.Department.ToUpper())).ToList();
            }

            var result = listquery;
            // Get equal account
            if (!string.IsNullOrEmpty(query.Account))
            {
                result = result.Where(x => CutName(x.Name) == query.Account).ToList();
            }

            return await Task.FromResult(result);
        }
        public string CutName(string name)
        {
            var arr = name.Split('|');
            return arr.LastOrDefault();
        }
        public async Task<List<EOfficeUserInfo>> GetEOfficeAccountsSharePoint()
        {
            // SSL
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            // HttpClient
            Uri uri = new Uri(Environment.GetEnvironmentVariable("BCM_WCF_ADDRESS"));
            var cookie = await GeteOfficeAuthCookie
            (
                Environment.GetEnvironmentVariable("BCM_WCF_ADDRESS"),
                Environment.GetEnvironmentVariable("BCM_LDAP_ACCOUNT"),
                Environment.GetEnvironmentVariable("BCM_LDAP_PASSWORD")
            );
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = uri })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                {
                    cookieContainer.Add(cookie);
                    var getResponse = await client.GetAsync($"_layouts/15/VuThao.Becamex.API/ApiUser.ashx?func=getUsersInGroup&gid=13");
                    if (getResponse.IsSuccessStatusCode)
                    {
                        EOfficeAccountsSharePoint result = JsonConvert.DeserializeObject<EOfficeAccountsSharePoint>(getResponse.Content.ReadAsStringAsync().Result);
                        return result.data;
                    }
                    else
                    {
                        return new List<EOfficeUserInfo>();
                    }
                }
            }
        }
        public class EOfficeAccountsSharePoint
        {
            public List<EOfficeUserInfo> data { get; set; }
        }
        public async Task<List<EOfficeUserNameOnly>> GetEofficeUserNameOnly()
        {
            var currentUsers = _context.Users.Select(x => x.UserName).ToList();
            //var result =  _context.EOfficeUserInfo.Select(x => new EOfficeUserNameOnly(x.Name.Substring(x.Name.LastIndexOf("|") + 1, x.Name.Length - x.Name.LastIndexOf("|") - 1), x.Title)).Where(t => currentUsers.Contains(t.UserName)).ToList();
            var result = _context.EOfficeUserInfo.AsQueryable().Select(x => new EOfficeUserNameOnly(x.Name.Substring(x.Name.LastIndexOf("|") + 1, x.Name.Length - x.Name.LastIndexOf("|") - 1), x.Title)).ToList();
            var result2 = result.Where(x => currentUsers.Contains(x.UserName));
            return await Task.FromResult(result2.ToList());
            
        }

        public Task<string> GetEofficeUserNameById(string Id)
        {
            var eOfficeList = _context.EOfficeUserInfo.AsQueryable().Select(x => new EOfficeUserNameOnly(x.Name.Substring(x.Name.LastIndexOf("|") + 1, x.Name.Length - x.Name.LastIndexOf("|") - 1), x.Title)).ToList();
            string userFullName = eOfficeList.FirstOrDefault(t=>t.UserName == Id).UserFullName;
            if (String.IsNullOrEmpty(userFullName)) userFullName = Id;
            return Task.FromResult(userFullName);

        }


        public class EOfficeUserNameOnly
        {
            public EOfficeUserNameOnly(string username, string fullname)
            {
                this.UserName = username;
                this.UserFullName = fullname;
            }
            public string UserName { get; set; }
            public string UserFullName { get; set; }
        }
        public bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }





    }
}