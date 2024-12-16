using MongoDB.Driver;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Mongo;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Common;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202211221536_ValueLabel : MongoMigration
    {
        private readonly IMongoCollection<FormInstance> Collection;        
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly IConfiguration configuration;
        public override int Version => 1;

        public M_202211221536_ValueLabel(IConfiguration configuration, SReportsContext dbContext)
        {
            this.configuration = configuration;
            thesaurusDAL = new ThesaurusDAL(dbContext, configuration);
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<FormInstance>(MongoCollectionNames.FormInstance);
        }

        protected override void Up()
        {
            CreateFieldValueLabels();
            CreateValueLabelIndex();
            FillRadioFieldsValueLabels(LanguageConstants.EN).Wait();
        }

        protected override void Down()
        {
            Collection.Indexes.DropOne("Fields_ValueLabel");
            bool updated = Collection.UpdateMany(
                Builders<FormInstance>.Filter.Empty,
                Builders<FormInstance>.Update.Unset("Fields.$[].ValueLabel"))
                .IsAcknowledged;

            if (!updated)
                throw new InvalidOperationException("MongoDB Migration Downgrade not successful while removing ValueLabel field from FormInstance!");
        }

        // ---------- Helper Methods ----------

        private void CreateValueLabelIndex()
        {
            // MongoDB can have only one full text index per Collection
            IndexKeysDefinition<FormInstance> key = Builders<FormInstance>.IndexKeys.Text("Fields.ValueLabel");
            Collection.Indexes.CreateOne(new CreateIndexModel<FormInstance>(key, new CreateIndexOptions() { Name = "Fields_ValueLabel" }));  
        }

        private void CreateFieldValueLabels()
        {
            var filterDefinition = Builders<FormInstance>.Filter.Empty;

            // Creates a ValueLabel field in every FormInstance.Fields and fills it with the first value of Fields.Value
            var pipeline = new EmptyPipelineDefinition<FormInstance>()
                .AppendStage<FormInstance, FormInstance, FormInstance>("{$set: {Fields: { $map: { input: '$Fields', 'in': { $mergeObjects: [ '$$this', { ValueLabel: '$$this.Value'}]}}}}}");

            var update = Builders<FormInstance>.Update.Pipeline(pipeline);

            var result = Collection.UpdateMany(filterDefinition, update).IsAcknowledged;
            Console.WriteLine(result);
        }

        private async Task FillRadioFieldsValueLabels(string activeLanguage)
        {
            var findOptions = new FindOptions<FormInstance, FormInstance>() { BatchSize = 5000 };
            var radioFilter = Builders<FormInstance>.Filter.Where(x => x.FieldInstances.Any(y => y.Type == "radio"));
            var instancesToWrite = new List<WriteModel<FormInstance>>();

            using (var cursor = await Collection.FindAsync(radioFilter, findOptions))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    List<int> thesaurusIds = new List<int>();

                    //Collecting all Thesaurus Values to Perform a single SQL Query
                    foreach (FormInstance formInstance in batch)
                    {                        
                        foreach (FieldInstance radioFieldValue in formInstance.FieldInstances.Where(y => y.Type == "radio").ToList())
                        {
                            List<int> Ids = TryParseToInt(TryGetAllDifferentValues(radioFieldValue));
                            if (Ids != null)
                                thesaurusIds = thesaurusIds.Union(Ids).ToList();                          
                        }
                    }

                    //Performing the Query
                    var thesaurusEntries = thesaurusDAL.GetByIdsList(thesaurusIds);

                    //Substituting LabelValue(s) with correspondent Thesaurus Term
                    foreach (var thesaurusEntry in thesaurusEntries)
                    {
                        batch.SelectMany(forminstance => forminstance.FieldInstances)
                            .Where(fieldValue => fieldValue.Type == "radio" && TryParseToInt(TryGetAllDifferentValues(fieldValue)).Contains(thesaurusEntry.ThesaurusEntryId)).ToList()
                            .ForEach(radioFieldValue => {
                                if(radioFieldValue.FieldInstanceValues.HasAnyFieldInstanceValue())
                                {
                                    for (int i = 0; i < radioFieldValue.FieldInstanceValues.Count; i++)
                                    {
                                        if (!string.IsNullOrWhiteSpace(radioFieldValue.FieldInstanceValues[i].ValueLabel))
                                        {
                                            string valueLabel = radioFieldValue.FieldInstanceValues[i].ValueLabel;
                                            if (int.TryParse(valueLabel, out int intValueLabel) && intValueLabel == thesaurusEntry.ThesaurusEntryId)
                                                radioFieldValue.FieldInstanceValues[i].ValueLabel = thesaurusEntry.GetPreferredTermByActiveLanguage(activeLanguage);
                                        }
                                    }
                                }
                            });
                    }

                    //Replacing the modified FormInstances into MongoDB Collection
                    foreach(FormInstance formInstance in batch)
                    {
                        var replaceFilter = Builders<FormInstance>.Filter.Eq(x => x.Id, formInstance.Id);
                        instancesToWrite.Add(new ReplaceOneModel<FormInstance>(replaceFilter, formInstance)); 
                    }
                    if (instancesToWrite.Any())
                    {
                        var result = await Collection.BulkWriteAsync(instancesToWrite);
                        if (!result.IsAcknowledged)
                            throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {batch.Count()}");
                    }
                   
                     
                    instancesToWrite.Clear();
                }
            }
        }

        private List<int> TryParseToInt(List<string> values)
        {
            List<int> result = new List<int>();
            if (values != null && values.Count > 0)
            {
                foreach (string val in values)
                {
                    if (val != null && int.TryParse(val, out int intValue))
                        result.Add(intValue);
                }
            }
            return result;
        }

        private List<string> TryGetAllDifferentValues(FieldInstance fieldInstance)
        {
            List<string> result = new List<string>();
            if (fieldInstance.FieldInstanceValues.HasAnyFieldInstanceValue())
            {
                result = fieldInstance.FieldInstanceValues.Select(x => x.Value).Distinct().ToList();
            }
            return result;
        }
    }
}

