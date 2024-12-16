using Microsoft.EntityFrameworkCore;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.OutsideUser;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.SqlDomain.Implementations
{
    public class OutsideUserDAL : IOutsideUserDAL
    {
        private readonly SReportsContext context;
        public OutsideUserDAL(SReportsContext context)
        {
            this.context = context;
        }

        public void Delete(int id)
        {
            OutsideUser formDb = context.OutsideUsers.FirstOrDefault(x => x.OutsideUserId == id);
            if (formDb != null)
            {
                formDb.Delete();
                context.SaveChanges();
            }
        }


        public List<OutsideUser> GetAllByIds(List<int> ids)
        {
            return context.OutsideUsers.Include(x => x.OutsideUserAddress).Where(x => ids.Contains(x.OutsideUserId)).ToList();
        }

        public OutsideUser GetById(int id)
        {
            return context.OutsideUsers.Include(x => x.OutsideUserAddress).FirstOrDefault(x => x.OutsideUserId == id);
        }

        public int InsertOrUpdate(OutsideUser user)
        {
            if (user.OutsideUserId == 0)
            {
                context.OutsideUsers.Add(user);
            }
            else
            {
                OutsideUser dbUser = this.GetById(user.OutsideUserId);
                dbUser.Copy(user);
            }
            context.SaveChanges();
            return user.OutsideUserId;

        }

    }
}
