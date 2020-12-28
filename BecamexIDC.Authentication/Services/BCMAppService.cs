using System;
using System.Threading.Tasks;
using BecamexIDC.Authentication.Data;
using BecamexIDC.Authentication.Models.Entities.BCMAppModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BecamexIDC.Authentication.Services
{
    public interface IBCMAppService
    {
        Task<BCMAppUserInfo> GetBCMAppUserInfo(BCMAppUserQuery query);
    }
    public class BCMAppService : IBCMAppService
    {
        private readonly BCMAppDbContext _context;
        private readonly IConfiguration _configuration;

        public BCMAppService(BCMAppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<BCMAppUserInfo> GetBCMAppUserInfo(BCMAppUserQuery query)
        {
            // user
            var user = new ViewUsers();
            var userInfo = new BCMAppUserInfo();
            if (query.username != null)
            {
                user = await _context.ViewUsers.FirstOrDefaultAsync(u => u.username == query.username);
            }
            // avatar and cover url
            if (user != null)
            {
                if (user.avatar != null)
                {
                    user.avatar = Environment.GetEnvironmentVariable("BCM_SOCIAL_NETWORK") + "media/user/avatar/" + user.avatar;
                }
                else
                {
                    user.avatar = Environment.GetEnvironmentVariable("BCM_SOCIAL_NETWORK") + $"media/user/avatar/default-{user.gender}-avatar.png";
                }
                if (user.cover != null)
                {
                    user.cover = Environment.GetEnvironmentVariable("BCM_SOCIAL_NETWORK") + "media/user/cover/" + user.cover;
                }
                else
                {
                    user.cover = Environment.GetEnvironmentVariable("BCM_SOCIAL_NETWORK") + "media/user/cover/default-cover-user.png";
                }
                // department
                var department = new ViewDepartments();
                if (user.department_id != null)
                {
                    department = await _context.ViewDepartments.FirstOrDefaultAsync(d => d.code == user.department_id);
                }

                userInfo.name = user.name;
                userInfo.username = user.username;
                userInfo.email = user.email;
                userInfo.avatar = user.avatar;
                userInfo.cover = user.cover;
                userInfo.created_at = user.created_at;
                userInfo.updated_at = user.updated_at;
                userInfo.code = user.code;
                userInfo.staff_id = user.staff_id;
                userInfo.timeline_id = user.timeline_id == null ? 0 : user.timeline_id;
                userInfo.department = department;
            }
            return await Task.FromResult(userInfo);
        }
    }
}