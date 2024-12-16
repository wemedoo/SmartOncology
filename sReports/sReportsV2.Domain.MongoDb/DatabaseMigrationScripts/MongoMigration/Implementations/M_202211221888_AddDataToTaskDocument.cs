using MongoDB.Driver;
using System.Linq;
using sReportsV2.Domain.Mongo;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Sql.Entities.TaskEntry;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Constants;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202211221888_AddDataToTaskDocument : MongoMigration
    {
        private readonly IMongoCollection<Form> Collection;        
        private readonly ITaskDAL taskDAL;
        private readonly ICodeDAL codeDAL;
        private readonly IConfiguration configuration;
        public override int Version => 4;

        public M_202211221888_AddDataToTaskDocument(IConfiguration configuration, SReportsContext dbContext)
        {
            this.configuration = configuration;
            taskDAL = new TaskDAL(dbContext);
            codeDAL = new CodeDAL(dbContext, configuration);
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);
        }

        protected override void Up()
        {
            AddDataToTaskDocumentTable();
        }

        protected override void Down()
        {
        }

        // ---------- Helper Methods ----------

        private void AddDataToTaskDocumentTable()
        {
            foreach (var form in Collection.AsQueryable().Where(x => !x.IsDeleted && x.State.Equals(Common.Enums.FormDefinitionState.ReadyForDataCapture)))
            {
                int codeId = InsertCode(form.ThesaurusId);
                InsertTaskDocument(codeId, form);
            }
        }

        private int InsertCode(int thesaurusId)
        {
            Code code = new Code()
            {
                ThesaurusEntryId = thesaurusId,
                CodeSetId = (int)CodeSetList.TaskDocument
            };

            return codeDAL.Insert(code);
        }

        private void InsertTaskDocument(int codeId, Form form)
        {
            TaskDocument taskDocument = new TaskDocument()
            {
                TaskDocumentCD = codeId,
                FormId = form.Id,
                FormTitle = form.Title
            };

            taskDAL.InsertTaskDocument(taskDocument);
        }
    }
}

