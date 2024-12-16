using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Cache.Singleton;
using sReportsV2.Domain.Sql.Entities.RoleEntry;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.DTOs.AccessManagment.DataIn;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Controllers
{
    public class RoleAdministrationController : BaseController
    {
        private readonly IPositionPermissionBLL positionPermissionBLL;
        private readonly IMapper Mapper;
        public RoleAdministrationController(IPositionPermissionBLL positionPermissionBLL, 
            IMapper mapper,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.positionPermissionBLL = positionPermissionBLL;
            Mapper = mapper;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult GetAll()
        {
            return View();
        }

        [SReportsAuthorize]
        public ActionResult ReloadTable(DataIn dataIn)
        {
            var result = GetAllRoles(dataIn);
            return PartialView("RoleEntryTable", result);
        }

        //[SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        //public ActionResult Create()
        //{
        //    return View("Edit");
        //}

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        public ActionResult Edit(int roleCD)
        {
            return GetEditViewResponse(roleCD);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult View(int roleCD)
        {
            return GetEditViewResponse(roleCD);
        }

        private ActionResult GetEditViewResponse(int roleCD)
        {
            PositionDataOut result = new PositionDataOut
            {
                Position = SingletonDataContainer.Instance.GetCode(roleCD),
                Modules = positionPermissionBLL.GetModules(),
                Permissions = positionPermissionBLL.GetPermissionsForRole(roleCD)
            };
            return View(EndpointConstants.Edit, result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Edit(PositionDataIn roleDataIn)
        {
            var response = positionPermissionBLL.InsertOrUpdate(roleDataIn);
            UpdateUserCookie(userCookieData.Email);
            return Json(response);
        }

        private PaginationDataOut<CodeDataOut, DataIn> GetAllRoles(DataIn dataIn)
        {
            RoleFilter filterData = Mapper.Map<RoleFilter>(dataIn);
            List<CodeDataOut> roles = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Role);
            var result = new PaginationDataOut<CodeDataOut, DataIn>()
            {
                Count = roles.Count,
                DataIn = dataIn,
                Data = Mapper.Map<List<CodeDataOut>>(FilterRoles(filterData, roles))
            };

            return result;
        }

        private List<CodeDataOut> FilterRoles(RoleFilter roleFilter, List<CodeDataOut> roles)
        {
            IQueryable<CodeDataOut> result = roles.AsQueryable();

            if (roleFilter.ColumnName != null)
            {
                switch (roleFilter.ColumnName)
                {
                    case AttributeNames.PreferredTerm:
                        if (roleFilter.IsAscending)
                        {
                            result = result
                                .OrderBy(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage))
                                .Skip((roleFilter.Page - 1) * roleFilter.PageSize)
                                .Take(roleFilter.PageSize);
                        }
                        else
                        {
                            result = result
                                .OrderByDescending(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage))
                                .Skip((roleFilter.Page - 1) * roleFilter.PageSize)
                                .Take(roleFilter.PageSize);
                        }
                        break;
                    case AttributeNames.Definition:
                        if (roleFilter.IsAscending)
                        {
                            result = result
                                .OrderBy(x => x.Thesaurus.GetDefinitionByTranslationOrDefault(userCookieData.ActiveLanguage))
                                .Skip((roleFilter.Page - 1) * roleFilter.PageSize)
                                .Take(roleFilter.PageSize);
                        }
                        else
                        {
                            result = result
                                .OrderByDescending(x => x.Thesaurus.GetDefinitionByTranslationOrDefault(userCookieData.ActiveLanguage))
                                .Skip((roleFilter.Page - 1) * roleFilter.PageSize)
                                .Take(roleFilter.PageSize);
                        }
                        break;
                    default:
                        result = result
                            .Skip((roleFilter.Page - 1) * roleFilter.PageSize)
                            .Take(roleFilter.PageSize);
                        break;
                }
            }
            else
            {
                result = result
                            .Skip((roleFilter.Page - 1) * roleFilter.PageSize)
                            .Take(roleFilter.PageSize);
            }

            return result.ToList();
        }
    }
}