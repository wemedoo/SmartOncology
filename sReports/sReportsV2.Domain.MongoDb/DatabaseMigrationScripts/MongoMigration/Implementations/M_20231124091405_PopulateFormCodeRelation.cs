using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;
using System.Linq;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_20231124091405_PopulateFormCodeRelation : MongoMigration
    {
        private readonly IMongoCollection<Form> Collection;
        private readonly IFormCodeRelationDAL formCodeRelationDAL;
        private readonly ICodeDAL codeDAL;
        private readonly IConfiguration configuration;
        public override int Version => 7;

        public M_20231124091405_PopulateFormCodeRelation(IConfiguration configuration, SReportsContext dbContext)
        {
            this.configuration = configuration;
            formCodeRelationDAL = new FormCodeRelationDAL(dbContext);
            codeDAL = new CodeDAL(dbContext, configuration);
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);
        }

        protected override void Up()
        {
            PopulateFormCodeRelation();
        }

        protected override void Down()
        {

        }

        private void PopulateFormCodeRelation()
        {
            foreach (var form in Collection.AsQueryable().Where(x => !x.IsDeleted))
            {
                int codeId = InsertCode(form.ThesaurusId);
                InsertFormCodeRelation(codeId, form.Id);
            }
        }

        private int InsertCode(int thesaurusId)
        {
            Code code = new Code()
            {
                ThesaurusEntryId = thesaurusId,
                CodeSetId = (int)CodeSetList.Document
            };

            return codeDAL.Insert(code);
        }

        private void InsertFormCodeRelation(int codeId, string formId)
        {
            FormCodeRelation formCodeRelation = new FormCodeRelation()
            {
                CodeCD = codeId,
                FormId = formId
            };

            formCodeRelationDAL.InsertFormCodeRelation(formCodeRelation);
        }
    }
}
