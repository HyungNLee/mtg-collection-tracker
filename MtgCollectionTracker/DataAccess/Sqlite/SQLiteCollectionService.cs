using System.Data.SQLite;

using Dapper;

using DataAccess.Models;
using DataAccess.Services;

using Serilog;

namespace DataAccess.Sqlite
{
    public class SQLiteCollectionService : ICollectionService
    {
        public SQLiteCollectionService()
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: Constructor");

            SQLiteDatabaseCreator.CreateDatabaseIfNotExists();
        }

        public async Task<int> AddCollectionAsync(AddCollectionRequest request)
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: {nameof(AddCollectionAsync)}");

            var sql = @"
                insert into [Collection] (
                    [Name],
                    IsDeck,
                    MainboardId,
                    SideboardId
                )
                values (
                    @Name,
                    @IsDeck,
                    @MainboardId,
                    @SideboardId
                );

                select last_insert_rowid();";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", request.Name);
                parameters.Add("@IsDeck", request.IsDeck);
                parameters.Add("@MainboardId", request.MainboardId);
                parameters.Add("@SideboardId", request.SideboardId);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var lastId = await dbConnection.QueryAsync<int>(
                    sql: sql,
                    param: parameters);

                return lastId.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCollectionService)}: {nameof(AddCollectionAsync)}");
                throw;
            }
        }

        public async Task<int> AddDeckSideboardAsync(int mainboardId)
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: {nameof(AddDeckSideboardAsync)}");

            var mainboardCollection = await GetCollectionAsync(mainboardId);
            if (mainboardCollection == null)
            {
                throw new InvalidOperationException($"No mainboard deck for Id: {mainboardId}.");
            }

            // Check if mainboard is a deck
            if (!mainboardCollection.IsDeck)
            {
                throw new InvalidOperationException("Mainboard must be a deck to create a sideboard.");
            }

            // Check if sideboard exists
            if (mainboardCollection.SideboardId != null)
            {
                throw new InvalidOperationException($"A sideboard already exists for collection Id: {mainboardId}.");
            }

            // Insert collection as a sideboard
            var sideboardRequest = new AddCollectionRequest
            {
                IsDeck = true,
                MainboardId = mainboardId,
                Name = $"{mainboardCollection.Name} - Sideboard"
            };
            var insertedSideboardCollectionId = await AddCollectionAsync(sideboardRequest);

            // Update mainboard with sideboard Id
            var sql = @"
                update [Collection]
                set SideboardId = @SideboardId
                where
                    Id = @Id;";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", mainboardId);
                parameters.Add("@SideboardId", insertedSideboardCollectionId);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                await dbConnection.ExecuteAsync(
                    sql: sql,
                    param: parameters);

                return insertedSideboardCollectionId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCollectionService)}: {nameof(AddDeckSideboardAsync)}");
                throw;
            }
        }

        public async Task<int> AddOwnedCardAsync(OwnedCardRequest request)
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: {nameof(AddOwnedCardAsync)}");

            var storedProcedure = @"
                insert into OwnedCard (CardPrintId, CollectionId, IsFoil)
                values (@CardPrintId, @CollectionId, @IsFoil);

                select last_insert_rowid();";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CardPrintId", request.CardPrintId);
                parameters.Add("@CollectionId", request.CollectionId);
                parameters.Add("@IsFoil", request.IsFoil);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var lastId = await dbConnection.QueryAsync<int>(
                    sql: storedProcedure,
                    param: parameters);

                return lastId.First();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCollectionService)}: {nameof(AddOwnedCardAsync)}");
                throw;
            }
        }

        public async Task DeleteOwnedCardsAsync(OwnedCardRequest request, int numberToDelete)
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: {nameof(DeleteOwnedCardsAsync)}");

            var sql = @"
                delete
                from OwnedCard
                where
                    Id in (
                        select
                            Id
                        from OwnedCard
                        where
                            CardPrintId = @CardPrintId
                            and CollectionId = @CollectionId
                            and IsFoil = @IsFoil
                        limit @NumberToDelete
                    )";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CardPrintId", request.CardPrintId);
                parameters.Add("@CollectionId", request.CollectionId);
                parameters.Add("@IsFoil", request.IsFoil);
                parameters.Add("@NumberToDelete", numberToDelete);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                await dbConnection.ExecuteAsync(
                    sql: sql,
                    param: parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCollectionService)}: {nameof(DeleteOwnedCardsAsync)}");
                throw;
            }
        }

        public async Task<CardCollection> GetCollectionAsync(int collectionId)
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: {nameof(GetCollectionAsync)}: ById");

            var sql = @"
                select
                    *
                from [Collection]
                where [Id] = @Id;";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", collectionId);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                return await dbConnection.QueryFirstOrDefaultAsync<CardCollection>(
                    sql: sql,
                    param: parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCollectionService)}: {nameof(GetCollectionAsync)}: ById");
                throw;
            }
        }

        public async Task<CardCollection> GetCollectionAsync(string name)
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: {nameof(GetCollectionAsync)}: ByName");

            var sql = @"
                select
                    *
                from [Collection]
                where [Name] = @Name;";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                return await dbConnection.QueryFirstOrDefaultAsync<CardCollection>(
                    sql: sql,
                    param: parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCollectionService)}: {nameof(GetCollectionAsync)}: ByName");
                throw;
            }
        }

        public async Task<IEnumerable<CardCollection>> GetCollectionsAsync()
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: {nameof(GetCollectionsAsync)}");

            var sql = @"
                select
                    *
                from [Collection];";

            try
            {
                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var foundCollections = await dbConnection.QueryAsync<CardCollection>(
                    sql: sql);

                return foundCollections ?? Enumerable.Empty<CardCollection>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCollectionService)}: {nameof(GetCollectionsAsync)}");
                throw;
            }
        }

        public async Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsyncByCardId(int cardId)
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: {nameof(GetOwnedCardsAggregatesAsyncByCardId)}");

            var storedProcedure = @"
                select
                    cpd.CardId,
                    cpd.CardName,
                    ocs.CardPrintId,
                    cpd.SetId,
                    cpd.SetName,
                    ocs.CollectionId,
                    c.[Name] as [CollectionName],
                    ocs.IsFoil,
                    cpd.PictureUrl,
                    cpd.FlipPictureUrl,
                    ocs.[Count]
                from vw_OwnedCardSum as ocs
                inner join [Collection] as c on ocs.CollectionId = c.Id
                inner join vw_CardPrintDetails as cpd on ocs.CardPrintId = cpd.Id
                where cpd.CardId = @CardId;";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CardId", cardId);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var foundCollections = await dbConnection.QueryAsync<OwnedCardPrintAggregate>(
                    sql: storedProcedure,
                    param: parameters);

                return foundCollections ?? Enumerable.Empty<OwnedCardPrintAggregate>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCollectionService)}: {nameof(GetOwnedCardsAggregatesAsyncByCardId)}");
                throw;
            }
        }

        public async Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsyncByCollectionId(int collectionId)
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: {nameof(GetOwnedCardsAggregatesAsyncByCollectionId)}");

            var storedProcedure = @"
                select
                    cpd.CardId,
                    cpd.CardName,
                    ocs.CardPrintId,
                    cpd.SetId,
                    cpd.SetName,
                    ocs.CollectionId,
                    c.[Name] as [CollectionName],
                    ocs.IsFoil,
                    cpd.PictureUrl,
                    cpd.FlipPictureUrl,
                    ocs.[Count]
                from vw_OwnedCardSum as ocs
                inner join [Collection] as c on ocs.CollectionId = c.Id
                inner join vw_CardPrintDetails as cpd on ocs.CardPrintId = cpd.Id
                where ocs.CollectionId = @CollectionId;";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CollectionId", collectionId);

                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var foundCollections = await dbConnection.QueryAsync<OwnedCardPrintAggregate>(
                    sql: storedProcedure,
                    param: parameters);

                return foundCollections ?? Enumerable.Empty<OwnedCardPrintAggregate>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCollectionService)}: {nameof(GetOwnedCardsAggregatesAsyncByCollectionId)}");
                throw;
            }

        }

        public async Task<IEnumerable<OwnedCardExport>> GetOwnedCardsExportFormatAsync()
        {
            Log.Debug($"{nameof(SQLiteCollectionService)}: {nameof(GetOwnedCardsExportFormatAsync)}");

            var sql = @"
                select 
                    cd.[Name] as CardName,
                    s.[Name] as SetName,
                    c.[Name] as CollectionName,
                    c.IsDeck,
                    [IsFoil],
                    [Count]
                from [vw_OwnedCardSum] as ocs
                inner join [Collection] as c on ocs.CollectionId = c.Id
                inner join [CardPrint] as cp on ocs.CardPrintId = cp.Id
                inner join [Card] as cd on cp.CardId = cd.Id
                inner join [Set] as s on cp.SetId = s.Id
                order by c.Id;";

            try
            {
                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var foundCollections = await dbConnection.QueryAsync<OwnedCardExport>(sql: sql);

                return foundCollections ?? Enumerable.Empty<OwnedCardExport>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SQLiteCollectionService)}: {nameof(GetOwnedCardsExportFormatAsync)}");
                throw;
            }
        }
    }
}
