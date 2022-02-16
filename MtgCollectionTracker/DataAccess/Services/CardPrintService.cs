using System.Data;
using System.Data.SqlClient;

using Dapper;

using DataAccess.Models;

using Microsoft.Extensions.Options;

namespace DataAccess.Services
{
    public class CardPrintService : ICardPrintService
    {
        private readonly DataAccessConfig _config;

        public CardPrintService(IOptions<DataAccessConfig> config)
        {
            _config = config.Value;
        }

        public async Task<Card> GetCardAsync(string name)
        {
            var storedProcedure = "Card_SelectBy_Name";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", name);

            using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
            var foundCard = await dbConnection.QueryFirstOrDefaultAsync<Card>(
                sql: storedProcedure,
                param: parameters,
                commandType: CommandType.StoredProcedure);

            return foundCard;
        }

        public async Task<CardPrintDetail> GetCardPrintDetailAsync(int cardId, int setId)
        {
            var storedProcedure = "vw_CardPrintDetails_SelectBy_CardId_SetId";

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

        public async Task<IEnumerable<CardPrintDetail>> GetCardPrintDetailsAsync()
        {
            var storedProcedure = "vw_CardPrintDetails_Select";

            using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
            var foundCardPrints = await dbConnection.QueryAsync<CardPrintDetail>(
                sql: storedProcedure,
                commandType: CommandType.StoredProcedure);

            return foundCardPrints;
        }

        public async Task<Set> GetSetAsync(string name)
        {
            var storedProcedure = "Set_SelectBy_Name";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", name);

            using IDbConnection dbConnection = new SqlConnection(_config.ConnectionString);
            var foundSet = await dbConnection.QueryFirstOrDefaultAsync<Set>(
                sql: storedProcedure,
                param: parameters,
                commandType: CommandType.StoredProcedure);

            return foundSet;
        }

        public async Task<int> InsertCardAsync(string name)
        {
            var storedProcedure = "Card_Insert";

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

        public async Task<int> InsertCardPrintAsync(int cardId, int setId, string pictureUrl, string flipPictureUrl)
        {
            var storedProcedure = "CardPrint_Insert";

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

        public async Task<int> InsertSetAsync(string name)
        {
            var storedProcedure = "Set_Insert";

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
    }
}
