using System.Data;
using System.Data.SqlClient;

using Dapper;

using DataAccess.Models;

using Microsoft.Extensions.Options;

using Serilog;

namespace DataAccess.Services
{
    public class CardPrintService : ICardPrintService
    {
        private readonly DataAccessConfig _config;

        public CardPrintService(IOptions<DataAccessConfig> config)
        {
            Log.Debug($"{nameof(CardPrintService)}: Constructor");

            _config = config.Value;
        }

        public async Task<Card> GetCardAsync(string name)
        {
            Log.Debug($"{nameof(CardPrintService)}: {nameof(GetCardAsync)}");

            var storedProcedure = "Card_SelectBy_Name";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);

                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                var foundCard = await dbConnection.QueryFirstOrDefaultAsync<Card>(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return foundCard;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CardPrintService)}: {nameof(GetCardAsync)}");
                throw;
            }
        }

        public async Task<CardPrintDetail> GetCardPrintDetailAsync(int cardId, int setId)
        {
            Log.Debug($"{nameof(CardPrintService)}: {nameof(GetCardPrintDetailAsync)}");

            var storedProcedure = "vw_CardPrintDetails_SelectBy_CardId_SetId";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CardId", cardId);
                parameters.Add("@SetId", setId);

                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                var foundCardPrint = await dbConnection.QueryFirstOrDefaultAsync<CardPrintDetail>(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return foundCardPrint;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CardPrintService)}: {nameof(GetCardPrintDetailAsync)}");
                throw;
            }
        }

        public Task<CardPrintDetail> GetCardPrintDetailAsync(string cardName, string setName)
        {
            Log.Debug($"{nameof(CardPrintService)}: {nameof(GetCardPrintDetailAsync)}");

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CardPrintDetail>> GetCardPrintDetailsAsync()
        {
            Log.Debug($"{nameof(CardPrintService)}: {nameof(GetCardPrintDetailsAsync)}");

            var storedProcedure = "vw_CardPrintDetails_Select";

            try
            {
                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                var foundCardPrints = await dbConnection.QueryAsync<CardPrintDetail>(
                    sql: storedProcedure,
                    commandType: CommandType.StoredProcedure);

                return foundCardPrints;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CardPrintService)}: {nameof(GetCardPrintDetailsAsync)}");
                throw;
            }
        }

        public async Task<Set> GetSetAsync(string name)
        {
            Log.Debug($"{nameof(CardPrintService)}: {nameof(GetSetAsync)}");

            var storedProcedure = "Set_SelectBy_Name";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);

                using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
                var foundSet = await dbConnection.QueryFirstOrDefaultAsync<Set>(
                    sql: storedProcedure,
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return foundSet;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(CardPrintService)}: {nameof(GetSetAsync)}");
                throw;
            }
        }

        public async Task<int> InsertCardAsync(string name)
        {
            Log.Debug($"{nameof(CardPrintService)}: {nameof(InsertCardAsync)}");

            var storedProcedure = "Card_Insert";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);
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
                Log.Error(ex, $"{nameof(CardPrintService)}: {nameof(InsertCardAsync)}");
                throw;
            }
        }

        public async Task<int> InsertCardPrintAsync(int cardId, int setId, string pictureUrl, string flipPictureUrl)
        {
            Log.Debug($"{nameof(CardPrintService)}: {nameof(InsertCardPrintAsync)}");

            var storedProcedure = "CardPrint_Insert";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CardId", cardId);
                parameters.Add("@SetId", setId);
                parameters.Add("@PictureUrl", pictureUrl);
                parameters.Add("@FlipPictureUrl", flipPictureUrl);
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
                Log.Error(ex, $"{nameof(CardPrintService)}: {nameof(InsertCardPrintAsync)}");
                throw;
            }
        }

        public async Task<int> InsertSetAsync(string name)
        {
            Log.Debug($"{nameof(CardPrintService)}: {nameof(InsertSetAsync)}");

            var storedProcedure = "Set_Insert";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);
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
                Log.Error(ex, $"{nameof(CardPrintService)}: {nameof(InsertSetAsync)}");
                throw;
            }
        }
    }
}
