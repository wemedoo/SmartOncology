using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.DAL.MongoDb.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.MongoDb.Entities.Promp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.DAL.MongoDb.Implementations
{
    public class PromptConfigurationDAL : IPromptConfigurationDAL
    {
        private readonly IMongoCollection<PromptForm> Collection;

        public PromptConfigurationDAL()
        {
            IMongoDatabase mongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = mongoDatabase.GetCollection<PromptForm>(MongoCollectionNames.PromptForm);
        }

        public async Task<string> GetPrompt(PromptFormFilter promptFormFilter)
        {
            PromptForm promptForm = await GetPromptForm(promptFormFilter);
            string prompt = string.Empty;
            if (promptForm != null)
            {
                PromptFormVersion promptFormVersion = promptForm.GetPromptFormVersion(promptFormFilter.VersionId);

                if (string.IsNullOrEmpty(promptFormFilter.FieldId))
                {
                    prompt = promptFormVersion?.Prompt;
                }
                else
                {
                    prompt = promptFormVersion.PromptFields.Find(pf => pf.FieldId == promptFormFilter.FieldId)?.Prompt;
                }
            }

            return prompt;
        }

        public async Task<List<Version>> GetVersions(PromptFormFilter promptFormFilter)
        {
            PromptForm promptForm = await GetPromptForm(promptFormFilter);
            return promptForm?.PromptFormVersions?.Select(x => x.Version)?.ToList() 
                ?? new List<Version>() { 
                    new Version {
                        Major = 1
                    }    
            };
        }

        public async Task<PromptForm> GetPromptForm(PromptFormFilter promptFormFilter)
        {
            return await
                Collection.AsQueryable().FirstOrDefaultAsync(p => p.FormId == promptFormFilter.FormId && p.ProjectId == promptFormFilter.ProjectId).ConfigureAwait(false);
        }

        public async Task<PromptFormVersion> GetPromptFormVersion(PromptFormFilter promptFormFilter)
        {
            PromptFormVersion promptFormVersion = null;
            PromptForm promptForm = await GetPromptForm(promptFormFilter).ConfigureAwait(false);
            if (promptForm != null)
            {
                promptFormVersion = promptForm.GetPromptFormVersion(promptFormFilter.VersionId);
            }
            return promptFormVersion;
        }

        public async Task InsertOrUpdate(PromptForm promptForm)
        {
            if (string.IsNullOrEmpty(promptForm.Id))
            {
                await Collection.InsertOneAsync(promptForm);
            }
            else
            {
                var filter = Builders<PromptForm>.Filter.Where(s => s.Id.Equals(promptForm.Id));
                await Collection.ReplaceOneAsync(filter, promptForm);
            }
        }

        public async Task<List<PromptForm>> GetAll()
        {
            return await Collection.AsQueryable().ToListAsync();
        }
    }
}
