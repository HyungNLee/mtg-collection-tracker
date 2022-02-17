using System.Data;
using System.Data.SqlClient;

using Dapper;

using DataAccess.Models;

using Microsoft.Extensions.Options;

namespace DataAccess.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly DataAccessConfig _config;

        public CollectionService(IOptions<DataAccessConfig> config)
        {
            _config = config.Value;
        }

        public async Task<IEnumerable<CardCollection>> GetCollectionsAsync()
        {
            var storedProcedure = "Collection_Select";

            using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
            var foundCollections = await dbConnection.QueryAsync<CardCollection>(
                sql: storedProcedure,
                commandType: CommandType.StoredProcedure);

            return foundCollections ?? Enumerable.Empty<CardCollection>();
        }

        public async Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsync(int collectionId)
        {
            var storedProcedure = "ivw_OwnedCardSum_Details_SelectBy_CollectionId";

            var parameters = new DynamicParameters();
            parameters.Add("@CollectionId", collectionId);

            using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
            var foundCollections = await dbConnection.QueryAsync<OwnedCardPrintAggregate>(
                sql: storedProcedure,
                param: parameters,
                commandType: CommandType.StoredProcedure);

            return foundCollections ?? Enumerable.Empty<OwnedCardPrintAggregate>();
        }
    }
}
