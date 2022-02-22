using System.Data.SQLite;

using Dapper;

using DataAccess.Models;
using DataAccess.Services;

namespace DataAccess.Sqlite
{
    public class SQLiteCardPrintService : ICardPrintService
    {
        public SQLiteCardPrintService()
        {
            SQLiteDatabaseCreator.CreateDatabaseIfNotExists();
        }

        public async Task<Card> GetCardAsync(string name)
        {
            string sql = @"
                select
                    *
                from [Card]
                where [Name] = @Name;";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", name);

            using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
            var foundCard = await dbConnection.QueryFirstOrDefaultAsync<Card>(
                sql: sql,
                param: parameters);

            return foundCard;
        }

        public async Task<CardPrintDetail> GetCardPrintDetailAsync(int cardId, int setId)
        {
            var sql = @"
                select
                    *
                from vw_CardPrintDetails
                where
                    CardId = @CardId
                    and SetId = @SetId;";

            var parameters = new DynamicParameters();
            parameters.Add("@CardId", cardId);
            parameters.Add("@SetId", setId);

            using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
            var foundCardPrint = await dbConnection.QueryFirstOrDefaultAsync<CardPrintDetail>(
                sql: sql,
                param: parameters);

            return foundCardPrint;
        }

        public async Task<CardPrintDetail> GetCardPrintDetailAsync(string cardName, string setName)
        {
            var sql = @"
                select
                    *
                from vw_CardPrintDetails
                where
                    CardName = @CardName
                    and SetName = @SetName;";

            var parameters = new DynamicParameters();
            parameters.Add("@CardName", cardName);
            parameters.Add("@SetName", setName);

            using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
            var foundCardPrint = await dbConnection.QueryFirstOrDefaultAsync<CardPrintDetail>(
                sql: sql,
                param: parameters);

            return foundCardPrint;
        }

        public async Task<IEnumerable<CardPrintDetail>> GetCardPrintDetailsAsync()
        {
            var sql = @"
                select
                    *
                from vw_CardPrintDetails;";

            using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
            var foundCardPrints = await dbConnection.QueryAsync<CardPrintDetail>(sql: sql);

            return foundCardPrints;
        }

        public async Task<Set> GetSetAsync(string name)
        {
            var sql = @"
                select
                    *
                from [Set]
                where [Name] = @Name;";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", name);

            using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
            var foundSet = await dbConnection.QueryFirstOrDefaultAsync<Set>(
                sql: sql,
                param: parameters);

            return foundSet;
        }

        public async Task<int> InsertCardAsync(string name)
        {
            var sql = @"
                insert into [Card] ([Name])
                values (@Name);

                select last_insert_rowid();";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", name);

            using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
            var lastId = await dbConnection.QueryAsync<int>(
                sql: sql,
                param: parameters);

            return lastId.First();
        }

        public async Task<int> InsertCardPrintAsync(int cardId, int setId, string pictureUrl, string flipPictureUrl)
        {
            var sql = @"
                insert into CardPrint (CardId, SetId, PictureUrl, FlipPictureUrl)
                values (@CardId, @SetId, @PictureUrl, @FlipPictureUrl);

                select last_insert_rowid();";

            var parameters = new DynamicParameters();
            parameters.Add("@CardId", cardId);
            parameters.Add("@SetId", setId);
            parameters.Add("@PictureUrl", pictureUrl);
            parameters.Add("@FlipPictureUrl", flipPictureUrl);

            using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
            var lastId = await dbConnection.QueryAsync<int>(
                sql: sql,
                param: parameters);

            return lastId.First();
        }

        public async Task<int> InsertSetAsync(string name)
        {
            var sql = @"
                insert into [Set] ([Name])
                values (@Name);

                select last_insert_rowid();";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", name);

            using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
            var lastId = await dbConnection.QueryAsync<int>(
                sql: sql,
                param: parameters);

            return lastId.First();
        }
    }
}
