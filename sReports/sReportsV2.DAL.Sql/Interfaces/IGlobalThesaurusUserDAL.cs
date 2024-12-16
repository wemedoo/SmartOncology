using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using System.Collections.Generic;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IGlobalThesaurusUserDAL
    {
        GlobalThesaurusUser GetByEmail(string email);
        GlobalThesaurusUser GetById(int id);
        bool ExistByEmailAndSource(string email, int? sourceCD);
        GlobalThesaurusUser InsertOrUpdate(GlobalThesaurusUser user);
        bool IsValidUser(string username, string password, int? activeStatusCD);
        List<GlobalThesaurusUser> GetAll();
    }
}
