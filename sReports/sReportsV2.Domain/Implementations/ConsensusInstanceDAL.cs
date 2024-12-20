﻿using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Services.Implementations
{
    public class ConsensusInstanceDAL : IConsensusInstanceDAL
    {
        private readonly IMongoCollection<ConsensusInstance> Collection;

        public ConsensusInstanceDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<ConsensusInstance>(MongoCollectionNames.ConsensusInstance);
        }

        public List<ConsensusInstance> GetAllByConsensusId(string consensusId)
        {
            return Collection.AsQueryable().Where(x => x.ConsensusRef == consensusId).ToList();
        }

        public List<ConsensusInstance> GetAllByConsensusAndIteration(string consensusId, string iterationId)
        {
            return Collection.AsQueryable().Where(x => x.ConsensusRef == consensusId && x.IterationId == iterationId).ToList();
        }

        public ConsensusInstance GetByConsensusAndUserAndIteration(string consensusId, int userId, string iterationId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ConsensusRef == consensusId && x.UserId == userId && x.IterationId == iterationId);
        }


        public ConsensusInstance GetById(string id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id == id);
        }

        public ConsensusInstance InsertOrUpdate(ConsensusInstance consensusInstance)
        {
            consensusInstance = Ensure.IsNotNull(consensusInstance, nameof(consensusInstance));

            if (consensusInstance.Id == null)
            {
                consensusInstance.Copy(null);
                Collection.InsertOne(consensusInstance);
            }
            else
            {
                ConsensusInstance consensusForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(consensusInstance.Id));
                consensusInstance.Copy(consensusForUpdate);
                var filter = Builders<ConsensusInstance>.Filter.Eq(s => s.Id, consensusInstance.Id);
                _ = Collection.ReplaceOne(filter, consensusInstance).ModifiedCount;
            }

            return consensusInstance;
        }
    }
}
