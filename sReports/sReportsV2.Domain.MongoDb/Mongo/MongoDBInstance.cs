using MongoDB.Driver;
using System;

namespace sReportsV2.Domain.Mongo
{
    public sealed class MongoDBInstance
    {
        private static volatile MongoDBInstance instance;
        private static readonly object syncLock = new object();
        private static IMongoDatabase db;

        private MongoDBInstance()
        {
            // Constructor is kept private to prevent direct instantiation.
        }

        public static MongoDBInstance Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncLock)
                    {
                        if (instance == null)
                        {
                            instance = new MongoDBInstance();
                            InitializeDatabase();
                        }
                    }
                }

                return instance;
            }
        }

        private static void InitializeDatabase()
        {
            if (db == null)
            {
                var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(MongoConfiguration.ConnectionString));
                mongoClientSettings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;
                mongoClientSettings.SslSettings.CheckCertificateRevocation = false;

                MongoClient client = new MongoClient(mongoClientSettings);
                db = client.GetDatabase("sReports");
            }
        }

        public IMongoDatabase GetDatabase()
        {
            return db;
        }
    }
}
