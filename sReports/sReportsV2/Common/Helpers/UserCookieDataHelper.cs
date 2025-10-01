using AutoMapper;
using sReportsV2.Common.Constants;
using sReportsV2.Cache.Singleton;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace sReportsV2.Common.Helpers
{
    public static class UserCookieDataHelper
    {
        public static UserCookieData PrepareUserCookie(IServiceProvider serviceProvider, bool isEmail, string identifier)
        {
            var mapper = serviceProvider.GetService<IMapper>();
            var personnelDAL = serviceProvider.GetService<IPersonnelDAL>();
            Personnel personnelEntity;
            if (isEmail)
            {
                personnelEntity = personnelDAL.GetByEmail(identifier);
            }
            else
            {
                personnelEntity = personnelDAL.GetByUsername(identifier);
            }
            if (personnelEntity == null) throw new ArgumentNullException(nameof(identifier));
            
            UserCookieData userCookieData = mapper.Map<UserCookieData>(personnelEntity);
            userCookieData.PositionPermissions = mapper.Map<List<PositionPermissionDataOut>>(personnelDAL.GetPermissions(personnelEntity.PersonnelId));
            //IMPORTANT NOTE: Add for the purposes of #2849
            SetCustomProperties(userCookieData);

            return userCookieData;
        }

        //START IMPORTANT NOTE: Add for the purposes of #2849
        private static void SetCustomProperties(UserCookieData userCookieData)
        {
            userCookieData.Roles =
                userCookieData
                .PositionPermissions
                .GroupBy(p => p.PositionId)
                .Select(gp => gp.Key)
                .Select(positionId =>
                    new RoleDataOut
                    {
                        Name = SingletonDataContainer.Instance.GetCodePreferredTerm(positionId)
                    }
                )
                .ToList();
        }
        //END IMPORTANT NOTE: Add for the purposes of #2849

        public static UserCookieData PrepareUserCookieForThGlobal(IServiceProvider serviceProvider, string email)
        {
            var mapper = serviceProvider.GetService<IMapper>();
            var globalUserDAL = serviceProvider.GetService<IGlobalThesaurusUserDAL>();
            GlobalThesaurusUser userEntity = globalUserDAL.GetByEmail(email);
            UserCookieData userCookieData = mapper.Map<UserCookieData>(userEntity);
            userCookieData.ActiveLanguage = LanguageConstants.EN;
            return userCookieData;
        }
    }
}