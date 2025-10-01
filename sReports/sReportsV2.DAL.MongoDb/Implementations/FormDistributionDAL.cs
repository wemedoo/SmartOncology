using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
namespace sReportsV2.Domain.Services.Implementations
{
    public class FormDistributionDAL : IFormDistributionDAL
    {
        private readonly IMongoCollection<FormDistribution> Collection;
        public FormDistributionDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<FormDistribution>(MongoCollectionNames.FormDistribution);
        }

        public IQueryable<FormDistribution> GetAll(EntityFilter filterData)
        {
            return Collection.AsQueryable().Skip(filterData.GetHowManyElementsToSkip()).Take(filterData.PageSize);
        }


        public List<FormDistribution> GetAll()
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted).ToList();
        }

        public List<FormDistribution> GetAllVersionAndThesaurus()
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted).Select(x=> new FormDistribution() { VersionId = x.VersionId, ThesaurusId = x.ThesaurusId }).ToList();
        }

        public int GetAllCount()
        {
            return Collection.AsQueryable().Count();
        }

        public FormDistribution GetById(string id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(id));
        }

        public FormDistribution GetByThesaurusId(int id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(id));
        }

        public FormDistribution GetByThesaurusIdAndVersion(int id, string versionId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(id) && x.VersionId == versionId);
        }

        public FormDistribution InsertOrUpdate(FormDistribution formDistribution)
        {
            formDistribution = Ensure.IsNotNull(formDistribution, nameof(formDistribution));

            if (string.IsNullOrEmpty(formDistribution.Id))
            {
                formDistribution.Copy(null);
                Collection.InsertOne(formDistribution);
            }
            else
            {
                Update(formDistribution);
            }

            return formDistribution;
        }

        public int ReplaceThesaurus(ThesaurusMerge thesaurusMerge, UserData userData = null)
        {
            int i = 0;
            foreach (FormDistribution formDistribution in GetAll())
            {
                formDistribution.ReplaceThesauruses(thesaurusMerge);
                InsertOrUpdate(formDistribution);
                i++;
            }
            
            return i;
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return Collection.AsQueryable()
                                .Any(form => !form.IsDeleted && 
                                        form.Fields.Any(
                                            field => field.ThesaurusId == thesaurusId
                                            || field.ValuesAll.Any(val => val.Values.Any(v => v.ThesaurusId == thesaurusId)))
                                        || form.ThesaurusId == thesaurusId
                                );
        }

        private void Update(FormDistribution formDistribution)
        {
            FormDistribution forUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(formDistribution.Id));
            formDistribution.Copy(forUpdate);
            FilterDefinition<FormDistribution> filter = Builders<FormDistribution>.Filter.Eq(s => s.Id, formDistribution.Id);
            var result = Collection.ReplaceOne(filter, formDistribution).ModifiedCount;
            Console.WriteLine(result);
        }

        public FormFieldDistribution GetFormFieldDistribution(string formDistributionId, string fieldId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(formDistributionId))
                .Fields
                .Find(x => x.Id.Equals(fieldId));
        }
    }
}
