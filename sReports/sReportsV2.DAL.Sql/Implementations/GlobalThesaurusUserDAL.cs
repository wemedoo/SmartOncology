using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.SqlDomain.Implementations
{
    public class GlobalThesaurusUserDAL : IGlobalThesaurusUserDAL
    {
        private readonly SReportsContext context;
        public GlobalThesaurusUserDAL(SReportsContext context)
        {
            this.context = context;
        }

        public bool ExistByEmailAndSource(string email, int? sourceCD)
        {
            return context.GlobalThesaurusUsers.Any(x => x.Email.Equals(email) && x.SourceCD == sourceCD);
        }

        public GlobalThesaurusUser GetByEmail(string email)
        {
            return context.GlobalThesaurusUsers
                .Include("GlobalThesaurusUserRoles")
                .Include("GlobalThesaurusUserRoles.GlobalThesaurusRole")
                .FirstOrDefault(x => x.Email.Equals(email));
        }

        public GlobalThesaurusUser GetById(int id)
        {
            return context.GlobalThesaurusUsers
                .Include("GlobalThesaurusUserRoles")
                .Include("GlobalThesaurusUserRoles.GlobalThesaurusRole")
                .FirstOrDefault(x => x.GlobalThesaurusUserId == id);
        }

        public List<GlobalThesaurusUser> GetAll()
        {
            return context.GlobalThesaurusUsers
                .Include("GlobalThesaurusUserRoles")
                .Include("GlobalThesaurusUserRoles.GlobalThesaurusRole")
                .WhereEntriesAreActive()
                .ToList();
        }

        public GlobalThesaurusUser InsertOrUpdate(GlobalThesaurusUser user)
        {
            if (user.GlobalThesaurusUserId == 0)
            {
                context.GlobalThesaurusUsers.Add(user);
            }

            context.SaveChanges();

            return user;
        }

        public bool IsValidUser(string username, string password, int? activeStatusCD)
        {
            return context.GlobalThesaurusUsers.Any(x => x.Email.Equals(username) && x.Password.Equals(password) && x.StatusCD == activeStatusCD);
        }
    }
}
