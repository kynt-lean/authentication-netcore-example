using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BecamexIDC.Authentication.Data;
using BecamexIDC.Authentication.Helpers;
using BecamexIDC.Authentication.Models;
using BecamexIDC.Authentication.Models.ViewModels;
using BecamexIDC.Authentication.Services.ApiSmartInService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BecamexIDC.Authentication.Services
{
    public interface IPermissionService{
        Task<List<PermissionViewModel>>  GetAll();
        Task<OperationResult> SavePermission(List<PermissionViewModel> model, string roleId);
        Task<List<PermissionViewModel>> GetAllFunctionRole(string roleId);
        Task<bool> CheckPermission(string functionCode, string action, string[] roles);
        Task<List<Permissions>> GetPermissionByRole(string[] roles);
    }

    
    public class PermissionService : IPermissionService
    {
         private readonly AuthDbContext _context;
        private IMapper _mapper;
        private MapperConfiguration _configMapper;
        private OperationResult operationResult;
        private IFunctionApiService _functionService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionService(AuthDbContext context,MapperConfiguration configMapper,IMapper mapper,IFunctionApiService functionService, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _configMapper=configMapper;
            _mapper = mapper;
            _functionService = functionService;
            _roleManager =roleManager;
        }

        public async Task<List<PermissionViewModel>> GetAll()
        {
            return await _context.Permissions.ProjectTo<PermissionViewModel>(_configMapper).ToListAsync();
        }

        public async Task<OperationResult> SavePermission(List<PermissionViewModel> model, string roleId)
        {
            var permissions = _mapper.Map<List<PermissionViewModel>, List<Permissions>>(model);
            var oldPermission = _context.Permissions.Where(x => x.RoleId == roleId).ToList();
            if (oldPermission.Count > 0)
            {
                 _context.Permissions.RemoveRange(oldPermission);
            }
            foreach (var permission in permissions)
            {
                 _context.Permissions.Add(permission);
            }

            try
            {
                await _context.SaveChangesAsync();
                operationResult = new OperationResult() { Caption = "Success", Message = "Save Complete", Success = true};
            }
            catch (System.Exception ex)
            {
                operationResult = new OperationResult() { Caption = "Failed", Message = ex.ToString(), Success = false };
            }
           return await Task.FromResult(operationResult);
        }

         public async Task<List<PermissionViewModel>> GetAllFunctionRole(string roleId)
        {
           
            var functions =await _functionService.GetAllFunction();
            var module = await _functionService.GetAllModules();
            var permissions = _context.Permissions.Where(x=>x.RoleId == roleId).ToList();

            var query = from f in functions.Output
                        where f.Code != null
                        join p in permissions on f.Code equals p.FunctionCode into fp
                        join m in module.Output on f.ModuleId equals m.Id into md
                        from p in fp.DefaultIfEmpty()
                        from m in md.DefaultIfEmpty()
                        select new PermissionViewModel()
                        {
                            Id = f.Id,
                            FunctionCode = f.Code,
                            FunctionName = f.NameVi,
                            CheckRow = p!=null &&
                            p.CanCreate == true && p.CanDelete == true && p.CanRead == true && p.CanUpdate == true ? true : false,
                            CanCreate = p != null ? p.CanCreate : false,
                            CanDelete = p != null ? p.CanDelete : false,
                            CanRead = p != null ? p.CanRead : false,
                            CanUpdate = p != null ? p.CanUpdate : false,
                            ModuleName =m.NameVi,
                            ModuleId = m.Id,
                            ModulePermission = m.IsPermission
                        };

            var data = query.Distinct().Where(x => x.ModulePermission == true).OrderBy(x => x.ModuleId).ToList();


            return await Task.FromResult(data);
        }

        public async Task<bool> CheckPermission(string functionCode, string action, string[] roles)
        {
            var functions =await _functionService.GetAllFunction();
            var permissions = _context.Permissions.ToList();
            var query = from f in functions.Output
                        join p in permissions on f.Code equals p.FunctionCode
                        join r in _roleManager.Roles on p.RoleId equals r.Id
                        where roles.Contains(r.Name) && f.Code == functionCode
                        && ((p.CanCreate && action == "Create")
                        || (p.CanUpdate && action == "Update")
                        || (p.CanDelete && action == "Delete")
                        || (p.CanRead && action == "Read"))
                        select p;
            return await Task.FromResult( query.Any());
        }

        public async Task<List<Permissions>> GetPermissionByRole(string[] roles)
        {
            var functions =await _functionService.GetAllFunction();
            var permissions = _context.Permissions.ToList();
            var query = from f in functions.Output
                        join p in permissions on f.Code equals p.FunctionCode
                        join r in _roleManager.Roles on p.RoleId equals r.Id
                        where roles.Contains(r.Name) 
                        select p;
            return await Task.FromResult(query.ToList());
        }
    }
}