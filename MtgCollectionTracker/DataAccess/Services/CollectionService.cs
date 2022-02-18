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

        public async Task<int> AddCollectionAsync(AddCollectionRequest request)
        {
            var storedProcedure = "Collection_Insert";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", request.Name);
            parameters.Add("@IsDeck", request.IsDeck);
            parameters.Add("@MainboardId", request.MainboardId);
            parameters.Add("@SideboardId", request.SideboardId);
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
                // TODO: Logging
            }

            return parameters.Get<int>("@Id");
        }

        public async Task<int> AddDeckSideboardAsync(int mainboardId)
        {
            var storedProcedure = "Collection_Insert_Sideboard";

            var parameters = new DynamicParameters();
            parameters.Add("@MainboardId", mainboardId);
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
                // TODO: Logging
            }

            return parameters.Get<int>("@Id");
        }

        public async Task<int> AddOwnedCardAsync(OwnedCardRequest request)
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
                // TODO: Logging
            }

            return parameters.Get<int>("@Id");
        }

        public async Task DeleteOwnedCardsAsync(OwnedCardRequest request, int numberToDelete)
        {
            var storedProcedure = "OwnedCard_Delete_Multiple";

            var parameters = new DynamicParameters();
            parameters.Add("@CardPrintId", request.CardPrintId);
            parameters.Add("@CollectionId", request.CollectionId);
            parameters.Add("@IsFoil", request.IsFoil);
            parameters.Add("@NumberToDelete", numberToDelete);

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
                // TODO: Logging
            }
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

        public async Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsyncByCardId(int cardId)
        {
            var storedProcedure = "ivw_OwnedCardSum_Details_SelectBy_CardId";

            var parameters = new DynamicParameters();
            parameters.Add("@CardId", cardId);

            IEnumerable<OwnedCardPrintAggregate> foundCollections = null;
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                foundCollections = await dbConnection.QueryAsync<OwnedCardPrintAggregate>(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

            }

            return foundCollections ?? Enumerable.Empty<OwnedCardPrintAggregate>();
        }

        public async Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsyncByCollectionId(int collectionId)
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
