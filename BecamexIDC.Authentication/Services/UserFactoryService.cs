using AutoMapper;
using BecamexIDC.Authentication.Data;
using BecamexIDC.Authentication.Helpers;
using BecamexIDC.Authentication.Models.Entities;
using BecamexIDC.Authentication.Models.ViewModels;
using BecamexIDC.Authentication.Services.ApiSmartInService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BecamexIDC.Authentication.Services
{
    public interface IUserFactoryService
    {
        Task<List<UserFactoryViewModel>> GetFactoryByUser(string userName);
        Task<OperationResult> Add(string userName, List<UserFactoryViewModel> model);
    }
    public class UserFactoryService : IUserFactoryService
    {
        private readonly AuthDbContext _context;
        private readonly IFactoryApiService _factoryService;
        private IMapper _mapper;
        private MapperConfiguration _configMapper;
        private readonly UserManager<IdentityUser> _userManager;

        public UserFactoryService(AuthDbContext context, IFactoryApiService factoryService, IMapper mapper, MapperConfiguration configMapper,
                UserManager<IdentityUser> userManager)
        {
            _context = context;
            _factoryService = factoryService;
            _mapper = mapper;
            _configMapper = configMapper;
            _userManager = userManager;
        }

        public async Task<List<UserFactoryViewModel>> GetFactoryByUser(string userName)
        {
            try
            {
                //Get all FActory GRPC
                var factorys = await _factoryService.GetAllFactory();
                var currentUser = await _userManager.FindByNameAsync(userName);
                //Get UserFactory
                var currentFactoryByUserId = _context.UserFactory.Where(x => x.UserId == currentUser.Id).ToList();

                var userFactorys = new List<UserFactoryViewModel>();
                foreach (var factory in factorys.Output)
                {
                    var userFactory = new UserFactoryViewModel();

                    userFactory.UserId = currentUser.Id;
                    userFactory.FactoryId = factory.FactoryId;
                    userFactory.FactoryName = factory.FactoryName;

                    var active = currentFactoryByUserId.FirstOrDefault(x => x.FactoryId == factory.FactoryId);

                    userFactory.Active = active != null ? true : false;

                    userFactorys.Add(userFactory);
                }
                return userFactorys;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<OperationResult> Add(string userName, List<UserFactoryViewModel> model)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
            if (identityUser != null)
            {
                try
                {
                    //remove all user factory
                    var currentFactory = await _context.UserFactory.Where(x => x.UserId == identityUser.Id).ToListAsync();
                    if (currentFactory.Count > 0)
                    {
                        _context.RemoveRange(currentFactory);
                        await _context.SaveChangesAsync();
                    }

                    if (model.Count > 0)
                    {
                        var userFactory = _mapper.Map<List<UserFactory>>(model);
                        _context.UserFactory.AddRange(userFactory);
                        var add = await _context.SaveChangesAsync();                        
                    }

                    return new OperationResult
                    {
                        Success = true,
                        Message = "Add user factory Complete ",
                        Caption = "Success"
                    };
                }
                catch (Exception ex)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = ex.ToString(),
                        Caption = "Error"
                    };
                }
                
            }
            return new OperationResult
            {
                Success = false,
                Message = "Add user factory failed ",
                Caption = "Error"
            };
            
        }
    }
}
