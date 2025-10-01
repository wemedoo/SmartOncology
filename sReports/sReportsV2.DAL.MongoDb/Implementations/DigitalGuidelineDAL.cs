using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities;
using sReportsV2.Domain.Entities.DigitalGuideline;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class DigitalGuidelineDAL : IDigitalGuidelineDAL
    {
        private readonly IMongoCollection<Guideline> Collection;

        public DigitalGuidelineDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<Guideline>(MongoCollectionNames.DigitalGuideline);
        }
        public async Task<Guideline> GetByIdAsync(string id)
        {
            return await Collection.Find(x => x.Id.Equals(id)).SingleAsync().ConfigureAwait(false);
        }

        public Guideline GetById(string id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => !x.IsDeleted && x.Id.Equals(id));
        }

        public async Task<Guideline> InsertOrUpdateAsync(Guideline guideline, int userId)
        {
            if (string.IsNullOrEmpty(guideline.Id))
            {
                guideline.Copy(null);
                guideline.InitializeVersionIfMissing();
                guideline.UserId = userId;

                await Collection.InsertOneAsync(guideline).ConfigureAwait(false);
            }
            else
            {
                Guideline guidelineForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(guideline.Id));
                guidelineForUpdate.DoConcurrencyCheck(guideline.LastUpdate.Value);
                guideline.Copy(guidelineForUpdate);
                var filter = Builders<Guideline>.Filter.Eq(s => s.Id, guideline.Id);
                _ = await Collection.ReplaceOneAsync(filter, guideline).ConfigureAwait(false);
            }

            return guideline;
        }

        public List<Guideline> GetAll(GuidelineFilter filter)
        {
            IQueryable<Guideline> result = GetGuidelineFiltered(filter);

            if (filter.ColumnName != null)
                result = SortByField(result, filter);
            else 
                result = result.OrderByDescending(x => x.EntryDatetime)
                   .Skip(filter.GetHowManyElementsToSkip())
                   .Take(filter.PageSize)
                   .Select(x => new Guideline()
                   {
                       Id = x.Id,
                       Version = x.Version,
                       LastUpdate = x.LastUpdate,
                       EntryDatetime = x.EntryDatetime,
                       Title = x.Title,
                   });

            return result.ToList();
        }

        public List<Guideline> GetAllByThesaurus(GuidelineFilter filter)
        {
            IQueryable<Guideline> result = GetGuidelineByThesaurusId(filter);

            if (filter.ColumnName != null)
                result = SortByField(result, filter);
            else
                result = result.OrderByDescending(x => x.EntryDatetime)
                   .Skip(filter.GetHowManyElementsToSkip())
                   .Take(filter.PageSize)
                   .Select(x => new Guideline()
                   {
                       Id = x.Id,
                       Version = x.Version,
                       LastUpdate = x.LastUpdate,
                       EntryDatetime = x.EntryDatetime,
                       Title = x.Title,
                       GuidelineElements = x.GuidelineElements,
                       ThesaurusId = x.ThesaurusId,
                   });

            return result.ToList();
        }

        public List<Guideline> GetAll()
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted)
               .OrderByDescending(x => x.EntryDatetime)
               .ToList();
        }

        public List<Guideline> SearchByTitle(string title)
        {
            return Collection.AsQueryable().Where(x => x.Title.ToUpper().Contains(title.ToUpper()))
               .OrderByDescending(x => x.EntryDatetime)
               .ToList();
        }

        public int GetAllCount(GuidelineFilter filter)
        {
            if(filter.ThesaurusId != 0)
                return this.GetGuidelineByThesaurusId(filter).Count();

            return this.GetGuidelineFiltered(filter).Count();
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            Guideline forDelete = this.GetById(id);
            Entity.DoConcurrencyCheckForDelete(forDelete);
            forDelete.DoConcurrencyBeforeDeleteCheck(lastUpdate);

            var filter = Builders<Guideline>.Filter.Eq(x => x.Id, id);
            var update = Builders<Guideline>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public Tuple<List<GuidelineEdgeElementData>, List<GuidelineEdgeElementData>> GetEdges(string nodeId, string guidelineId) 
        {
            List<GuidelineEdgeElementData> guidelineNextEdges = new List<GuidelineEdgeElementData>();
            List<GuidelineEdgeElementData> guidelinePreviusEdges = new List<GuidelineEdgeElementData>();
            foreach (GuidelineElementData item in Collection.AsQueryable().Where(x => !x.IsDeleted && x.Id.Equals(guidelineId)).Select(x => x.GuidelineElements.Edges.Select(c => c.Data)).FirstOrDefault()) 
            {
                GuidelineEdgeElementData castedElementData = (GuidelineEdgeElementData)item;
                if(castedElementData.Source == nodeId)
                    guidelineNextEdges.Add(castedElementData);
                if (castedElementData.Target == nodeId)
                    guidelinePreviusEdges.Add(castedElementData);
            }
            return Tuple.Create(guidelineNextEdges, guidelinePreviusEdges);
        }

        public Guideline GetGuidelineWithGreatestVersion(int thesaurusId)
        {
            List<Guideline> guidelines = this.GetFormsByThesaurusAndLanguageAndOrganization(thesaurusId);
            Guideline result = guidelines != null && guidelines.Count > 0 ? guidelines[0] : null;
            foreach (Guideline guideline in guidelines)
            {
                if (guideline.Version.IsVersionGreater(result.Version))
                {
                    result = guideline;
                }
            }

            return result;
        }

        public List<Guideline> GetFormsByThesaurusAndLanguageAndOrganization(int thesaurus)
        {
            return Collection.AsQueryable()
                           .Where(x => !x.IsDeleted
                               && x.ThesaurusId.Equals(thesaurus))
                           .Select(x => new Guideline() { Id = x.Id, Version = x.Version })
                           .ToList();
        }

        private IQueryable<Guideline> GetGuidelineFiltered(GuidelineFilter filter)
        {
            var result = Collection.AsQueryable()
                .Where(x => !x.IsDeleted);
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Title))
                    result = result.Where(x => x.Title.ToUpper().Contains(filter.Title));
                if (filter.Major != 0)
                    result = result.Where(x => x.Version.Major == filter.Major);
                if (filter.Minor != 0)
                    result = result.Where(x => x.Version.Minor == filter.Minor);
                if (filter.DateTimeTo != null)
                    result = result.Where(x => x.EntryDatetime < filter.DateTimeTo);
                if (filter.DateTimeFrom != null)
                    result = result.Where(x => x.EntryDatetime > filter.DateTimeFrom);
            }

            return result;
        }

        private IQueryable<Guideline> GetGuidelineByThesaurusId(GuidelineFilter filter)
        {
            var result = Collection.AsQueryable()
                .Where(x => !x.IsDeleted && x.ThesaurusId == filter.ThesaurusId && x.ThesaurusId != 0);

            return result;
        }

        private IQueryable<Guideline> SortByField(IQueryable<Guideline> result, GuidelineFilter filter)
        {
            switch (filter.ColumnName)
            {
                case AttributeNames.Version:
                    if (filter.IsAscending)
                        return result.OrderBy(x => x.Version.Major)
                                .ThenBy(x=>x.Version.Minor)
                                .Skip(filter.GetHowManyElementsToSkip())
                                .Take(filter.PageSize)
                                .Select(x => new Guideline()
                                {
                                    Id = x.Id,
                                    Version = x.Version,
                                    LastUpdate = x.LastUpdate,
                                    EntryDatetime = x.EntryDatetime,
                                    Title = x.Title,
                                });
                    else
                        return result.OrderByDescending(x => x.Version.Major)
                                .ThenByDescending(x=>x.Version.Minor)
                                .Skip(filter.GetHowManyElementsToSkip())
                                .Take(filter.PageSize)
                                .Select(x => new Guideline()
                                {
                                    Id = x.Id,
                                    Version = x.Version,
                                    LastUpdate = x.LastUpdate,
                                    EntryDatetime = x.EntryDatetime,
                                    Title = x.Title,
                                });
                default:
                    return SortTableHelper.OrderByField(result, filter.ColumnName, filter.IsAscending)
                                .Skip(filter.GetHowManyElementsToSkip())
                                .Take(filter.PageSize)
                                .Select(x => new Guideline()
                                {
                                    Id = x.Id,
                                    Version = x.Version,
                                    LastUpdate = x.LastUpdate,
                                    EntryDatetime = x.EntryDatetime,
                                    Title = x.Title,
                                });
            }
        }
    }
}
