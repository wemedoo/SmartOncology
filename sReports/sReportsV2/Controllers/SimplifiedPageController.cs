using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.CRF.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Controllers
{
    public class SimplifiedPageController : FormCommonController
    {

        public SimplifiedPageController(IUserBLL userBLL, 
            IOrganizationBLL organizationBLL, 
            ICodeBLL codeBLL, 
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IAsyncRunner asyncRunner, 
            IMapper mapper,      
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ICacheRefreshService cacheRefreshService) : 
            base(userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, asyncRunner, mapper, httpContextAccessor, serviceProvider, configuration, cacheRefreshService)
        {

        }

        protected List<TreeJsonDataOut> GetFormTreeData(List<Form> forms)
        {
            List<Form> formsForTree = new List<Form>();
            formsForTree.AddRange(forms);
            return GetTreeJson(formsForTree);
        }

        protected List<TreeJsonDataOut> GetTreeJson(List<Form> formsData)
        {
            List<TreeJsonDataOut> result = new List<TreeJsonDataOut>();
            foreach (Form formData in formsData)
            {
                TreeJsonDataOut treeJsonDataOut = new TreeJsonDataOut();
                treeJsonDataOut.text = formData.Title;
                treeJsonDataOut.nodes = formData.Chapters.Select(x => new TreeJsonDataOut() { text = x.Title, href = $"#@c.Id" }).ToList();

                result.Add(treeJsonDataOut);
            }

            return result;
        }
    }
}