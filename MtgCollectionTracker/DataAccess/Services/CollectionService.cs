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

        public async Task<int> AddOwnedCardAsync(AddOwnedCardRequest request)
        {
            var storedProcedure = "OwnedCard_Insert";

            var parameters = new DynamicParameters();
            parameters.Add("@CardPrintId", request.CardPrintId);
            parameters.Add("@CollectionId", request.CollectionId);
            parameters.Add("@IsFoil", request.IsFoil);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

            try
            {
                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                await dbConnection.ExecuteAsync(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

            }

            return parameters.Get<int>("@Id");
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
