using System.Data.SQLite;

using Dapper;

using DataAccess.Models;
using DataAccess.Services;

using Serilog;

namespace DataAccess.Sqlite
{
    public class SQLiteCardPrintService : ICardPrintService
    {
        public SQLiteCardPrintService()
        {
            Log.Debug($"{nameof(SQLiteCardPrintService)}: Constructor");

            SQLiteDatabaseCreator.CreateDatabaseIfNotExists();
        }

        public async Task<Card> GetCardAsync(string name)
        {
            Log.Debug($"{nameof(SQLiteCardPrintService)}: {nameof(GetCardAsync)}");

            string sql = @"
                select
                    *
                from [Card]
                where [Name] = @Name;";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var foundCard = await dbConnection.QueryFirstOrDefaultAsync<Card>(
                    sql: sql,
                    param: parameters);

                return foundCard;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCardPrintService)}: {nameof(GetCardAsync)}");
                throw;
            }
        }

        public async Task<CardPrintDetail> GetCardPrintDetailAsync(int cardId, int setId)
        {
            Log.Debug($"{nameof(SQLiteCardPrintService)}: {nameof(GetCardPrintDetailAsync)}");

            var sql = @"
                select
                    *
                from vw_CardPrintDetails
                where
                    CardId = @CardId
                    and SetId = @SetId;";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CardId", cardId);
                parameters.Add("@SetId", setId);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var foundCardPrint = await dbConnection.QueryFirstOrDefaultAsync<CardPrintDetail>(
                    sql: sql,
                    param: parameters);

                return foundCardPrint;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCardPrintService)}: {nameof(GetCardPrintDetailAsync)}");
                throw;
            }
        }

        public async Task<CardPrintDetail> GetCardPrintDetailAsync(string cardName, string setName)
        {
            Log.Debug($"{nameof(SQLiteCardPrintService)}: {nameof(GetCardPrintDetailAsync)}");

            var sql = @"
                select
                    *
                from vw_CardPrintDetails
                where
                    CardName = @CardName
                    and SetName = @SetName;";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CardName", cardName);
                parameters.Add("@SetName", setName);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var foundCardPrint = await dbConnection.QueryFirstOrDefaultAsync<CardPrintDetail>(
                    sql: sql,
                    param: parameters);

                return foundCardPrint;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCardPrintService)}: {nameof(GetCardPrintDetailAsync)}");
                throw;
            }
        }

        public async Task<IEnumerable<CardPrintDetail>> GetCardPrintDetailsAsync()
        {
            Log.Debug($"{nameof(SQLiteCardPrintService)}: {nameof(GetCardPrintDetailsAsync)}");

            var sql = @"
                select
                    *
                from vw_CardPrintDetails;";

            try
            {
                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var foundCardPrints = await dbConnection.QueryAsync<CardPrintDetail>(sql: sql);

                return foundCardPrints;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCardPrintService)}: {nameof(GetCardPrintDetailsAsync)}");
                throw;
            }
        }

        public async Task<Set> GetSetAsync(string name)
        {
            Log.Debug($"{nameof(SQLiteCardPrintService)}: {nameof(GetSetAsync)}");

            var sql = @"
                select
                    *
                from [Set]
                where [Name] = @Name;";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var foundSet = await dbConnection.QueryFirstOrDefaultAsync<Set>(
                    sql: sql,
                    param: parameters);

                return foundSet;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCardPrintService)}: {nameof(GetSetAsync)}");
                throw;
            }
        }

        public async Task<int> InsertCardAsync(string name)
        {
            Log.Debug($"{nameof(SQLiteCardPrintService)}: {nameof(InsertCardAsync)}");

            var sql = @"
                insert into [Card] ([Name])
                values (@Name);

                select last_insert_rowid();";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var lastId = await dbConnection.QueryAsync<int>(
                    sql: sql,
                    param: parameters);

                return lastId.First();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCardPrintService)}: {nameof(InsertCardAsync)}");
                throw;
            }
        }

        public async Task<int> InsertCardPrintAsync(int cardId, int setId, string pictureUrl, string flipPictureUrl)
        {
            Log.Debug($"{nameof(SQLiteCardPrintService)}: {nameof(InsertCardPrintAsync)}");

            var sql = @"
                insert into CardPrint (CardId, SetId, PictureUrl, FlipPictureUrl)
                values (@CardId, @SetId, @PictureUrl, @FlipPictureUrl);

                select last_insert_rowid();";

            try
            {
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
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCardPrintService)}: {nameof(InsertCardPrintAsync)}");
                throw;
            }
        }

        public async Task<int> InsertSetAsync(string name)
        {
            Log.Debug($"{nameof(SQLiteCardPrintService)}: {nameof(InsertSetAsync)}");

            var sql = @"
                insert into [Set] ([Name])
                values (@Name);

                select last_insert_rowid();";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var lastId = await dbConnection.QueryAsync<int>(
                    sql: sql,
                    param: parameters);

                return lastId.First();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCardPrintService)}: {nameof(InsertSetAsync)}");
                throw;
            }
        }
    }
}
