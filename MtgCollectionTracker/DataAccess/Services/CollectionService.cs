using System.Data;
using System.Data.SqlClient;

using Dapper;

using DataAccess.Models;

using Microsoft.Extensions.Options;

using Serilog;

namespace DataAccess.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly DataAccessConfig _config;

        public CollectionService(IOptions<DataAccessConfig> config)
        {
            Log.Debug($"{nameof(CollectionService)}: Constructor");

            _config = config.Value;
        }

        public async Task<int> AddCollectionAsync(AddCollectionRequest request)
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(AddCollectionAsync)}");

            var storedProcedure = "Collection_Insert";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", request.Name);
                parameters.Add("@IsDeck", request.IsDeck);
                parameters.Add("@MainboardId", request.MainboardId);
                parameters.Add("@SideboardId", request.SideboardId);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                await dbConnection.ExecuteAsync(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@Id");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CollectionService)}: {nameof(AddCollectionAsync)}");
                throw;
            }
        }

        public async Task<int> AddDeckSideboardAsync(int mainboardId)
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(AddDeckSideboardAsync)}");

            var storedProcedure = "Collection_Insert_Sideboard";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MainboardId", mainboardId);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                await dbConnection.ExecuteAsync(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@Id");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CollectionService)}: {nameof(AddDeckSideboardAsync)}");
                throw;
            }
        }

        public async Task<int> AddOwnedCardAsync(OwnedCardRequest request)
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(AddOwnedCardAsync)}");

            var storedProcedure = "OwnedCard_Insert";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CardPrintId", request.CardPrintId);
                parameters.Add("@CollectionId", request.CollectionId);
                parameters.Add("@IsFoil", request.IsFoil);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                await dbConnection.ExecuteAsync(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@Id");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CollectionService)}: {nameof(AddOwnedCardAsync)}");
                throw;
            }
        }

        public Task RemoveCollection(int collectionId)
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(RemoveCollection)}");

            throw new NotImplementedException();
        }

        public async Task DeleteOwnedCardsAsync(OwnedCardRequest request, int numberToDelete)
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(DeleteOwnedCardsAsync)}");

            var storedProcedure = "OwnedCard_Delete_Multiple";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CardPrintId", request.CardPrintId);
                parameters.Add("@CollectionId", request.CollectionId);
                parameters.Add("@IsFoil", request.IsFoil);
                parameters.Add("@NumberToDelete", numberToDelete);

                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                await dbConnection.ExecuteAsync(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CollectionService)}: {nameof(DeleteOwnedCardsAsync)}");
                throw;
            }
        }

        public Task<CardCollection> GetCollectionAsync(int collectionId)
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(GetCollectionAsync)}");

            throw new NotImplementedException();
        }

        public Task<CardCollection> GetCollectionAsync(string name)
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(GetCollectionAsync)}");

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CardCollection>> GetCollectionsAsync()
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(GetCollectionsAsync)}");

            try
            {
                var storedProcedure = "Collection_Select";

                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                var foundCollections = await dbConnection.QueryAsync<CardCollection>(
                    sql: storedProcedure,
                    commandType: CommandType.StoredProcedure);

                return foundCollections ?? Enumerable.Empty<CardCollection>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CollectionService)}: {nameof(GetCollectionsAsync)}");
                throw;
            }
        }

        public async Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsyncByCardId(int cardId)
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(GetOwnedCardsAggregatesAsyncByCardId)}");

            var storedProcedure = "ivw_OwnedCardSum_Details_SelectBy_CardId";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CardId", cardId);

                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                var foundCollections = await dbConnection.QueryAsync<OwnedCardPrintAggregate>(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return foundCollections ?? Enumerable.Empty<OwnedCardPrintAggregate>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CollectionService)}: {nameof(GetOwnedCardsAggregatesAsyncByCardId)}");
                throw;
            }
        }

        public async Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsyncByCollectionId(int collectionId)
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(GetOwnedCardsAggregatesAsyncByCollectionId)}");

            var storedProcedure = "ivw_OwnedCardSum_Details_SelectBy_CollectionId";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CollectionId", collectionId);

                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                var foundCollections = await dbConnection.QueryAsync<OwnedCardPrintAggregate>(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return foundCollections ?? Enumerable.Empty<OwnedCardPrintAggregate>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CollectionService)}: {nameof(GetOwnedCardsAggregatesAsyncByCollectionId)}");
                throw;
            }
        }

        public async Task<IEnumerable<OwnedCardExport>> GetOwnedCardsExportFormatAsync()
        {
            Log.Debug($"{nameof(CollectionService)}: {nameof(GetOwnedCardsExportFormatAsync)}");

            var storedProcedure = "ivw_OwnedCardSum_All_Export";

            try
            {
                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                var foundCollections = await dbConnection.QueryAsync<OwnedCardExport>(
                    sql: storedProcedure,
                    commandType: CommandType.StoredProcedure);

                return foundCollections ?? Enumerable.Empty<OwnedCardExport>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CollectionService)}: {nameof(GetOwnedCardsExportFormatAsync)}");
                throw;
            }
        }
    }
}
