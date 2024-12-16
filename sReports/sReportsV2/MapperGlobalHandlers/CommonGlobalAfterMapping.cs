using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;

namespace sReportsV2.MapperProfiles
{
    public class CommonGlobalAfterMapping<TDestination> : IMappingAction<object, TDestination> where TDestination : Entity
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public CommonGlobalAfterMapping(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration;
        }

        public void Process(object source, TDestination destination, ResolutionContext context)
        {
            try
            {
                var dbcontext = _httpContextAccessor.HttpContext;
                if (dbcontext != null)
                {
                    var user = dbcontext.Session.GetUserFromSession();
                    if (user != null && !_configuration.IsGlobalThesaurusRunning())
                    {
                        destination.CreatedById = user.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Error while setting created by property, error message: " + ex.Message);
            }
        }
    }
}
