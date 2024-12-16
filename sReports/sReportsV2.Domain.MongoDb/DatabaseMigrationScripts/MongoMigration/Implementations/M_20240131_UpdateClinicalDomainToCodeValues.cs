using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;
using System.Data.SqlClient;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_20240131_UpdateClinicalDomainToCodeValues : MongoMigration
    {
        private readonly IMongoCollection<Form> Collection;
        private readonly IConfiguration configuration;
        public override int Version => 13;

        public M_20240131_UpdateClinicalDomainToCodeValues(IConfiguration configuration)
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);
            this.configuration = configuration;
        }

        protected override void Up()
        {
            UpdateForms();
        }

        protected override void Down()
        {
        }

        // ---------- Helper Methods ----------

        private void UpdateForms()
        {
            try
            {
                string connectionString = configuration["Sql"];
                Builders<Form>.Filter.Eq(x => x.IsDeleted, false);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var document in Collection.AsQueryable().ToList())
                    {
                        for (int i = 0; i < document.DocumentProperties.ClinicalDomain.Count; i++)
                        {
                            int clinicalDomainId = (int)document.DocumentProperties.ClinicalDomain[i];

                            using (SqlCommand command = new SqlCommand("SELECT CodeId FROM ClinicalDomains WHERE ClinicalDomainId = @ClinicalDomainId", connection))
                            {
                                command.Parameters.AddWithValue("@ClinicalDomainId", clinicalDomainId);

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        int? codeId = reader["CodeId"] as int?;

                                        // Update the value in-memory
                                        document.DocumentProperties.ClinicalDomain[i] = codeId;
                                    }
                                }
                            }
                        }

                        // Update the document in the collection
                        var update = Builders<Form>.Update.Set(x => x.DocumentProperties.ClinicalDomain, document.DocumentProperties.ClinicalDomain);
                        Collection.UpdateOne(Builders<Form>.Filter.Eq(x => x.Id, document.Id), update);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }




    }
}