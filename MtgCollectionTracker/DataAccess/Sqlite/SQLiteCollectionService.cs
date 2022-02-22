using System.Data.SQLite;

using Dapper;

using DataAccess.Models;
using DataAccess.Services;

namespace DataAccess.Sqlite
{
    public class SQLiteCollectionService : ICollectionService
    {
        public SQLiteCollectionService()
        {
            SQLiteDatabaseCreator.CreateDatabaseIfNotExists();
        }

        public async Task<int> AddCollectionAsync(AddCollectionRequest request)
        {
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

            var parameters = new DynamicParameters();
            parameters.Add("@Name", request.Name);
            parameters.Add("@IsDeck", request.IsDeck);
            parameters.Add("@MainboardId", request.MainboardId);
            parameters.Add("@SideboardId", request.SideboardId);

            try
            {
                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var lastId = await dbConnection.QueryAsync<int>(
                    sql: sql,
                    param: parameters);

                return lastId.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // TODO: Logging
                throw ex;
            }

        }

        public async Task<int> AddDeckSideboardAsync(int mainboardId)
        {
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

            var parameters = new DynamicParameters();
            parameters.Add("@Id", mainboardId);
            parameters.Add("@SideboardId", insertedSideboardCollectionId);

            try
            {
                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                await dbConnection.ExecuteAsync(
                    sql: sql,
                    param: parameters);

                return insertedSideboardCollectionId;
            }
            catch (Exception ex)
            {
                // TODO: Logging
                throw ex;
            }
        }

        public async Task<int> AddOwnedCardAsync(OwnedCardRequest request)
        {
            var storedProcedure = @"
                insert into OwnedCard (CardPrintId, CollectionId, IsFoil)
                values (@CardPrintId, @CollectionId, @IsFoil);

                select last_insert_rowid();";

            var parameters = new DynamicParameters();
            parameters.Add("@CardPrintId", request.CardPrintId);
            parameters.Add("@CollectionId", request.CollectionId);
            parameters.Add("@IsFoil", request.IsFoil);

            try
            {
                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                var lastId = await dbConnection.QueryAsync<int>(
                    sql: storedProcedure,
                    param: parameters);

                return lastId.First();
            }
            catch (Exception ex)
            {
                // TODO: Logging
                throw ex;
            }
        }

        public async Task DeleteOwnedCardsAsync(OwnedCardRequest request, int numberToDelete)
        {
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

            var parameters = new DynamicParameters();
            parameters.Add("@CardPrintId", request.CardPrintId);
            parameters.Add("@CollectionId", request.CollectionId);
            parameters.Add("@IsFoil", request.IsFoil);
            parameters.Add("@NumberToDelete", numberToDelete);

            try
            {
                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                await dbConnection.ExecuteAsync(
                    sql: sql,
                    param: parameters);
            }
            catch (Exception ex)
            {
                // TODO: Logging
                throw ex;
            }
        }

        public async Task<CardCollection> GetCollectionAsync(int collectionId)
        {
            var sql = @"
                select
                    *
                from [Collection]
                where [Id] = @Id;";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", collectionId);

            using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
            return await dbConnection.QueryFirstOrDefaultAsync<CardCollection>(
                sql: sql,
                param: parameters);
        }

        public async Task<IEnumerable<CardCollection>> GetCollectionsAsync()
        {
            var sql = @"
                select
                    *
                from [Collection];";

            using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
            var foundCollections = await dbConnection.QueryAsync<CardCollection>(
                sql: sql);

            return foundCollections ?? Enumerable.Empty<CardCollection>();
        }

        public async Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsyncByCardId(int cardId)
        {
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

            var parameters = new DynamicParameters();
            parameters.Add("@CardId", cardId);

            IEnumerable<OwnedCardPrintAggregate> foundCollections = null;
            try
            {
                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                foundCollections = await dbConnection.QueryAsync<OwnedCardPrintAggregate>(
                    sql: storedProcedure,
                    param: parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return foundCollections ?? Enumerable.Empty<OwnedCardPrintAggregate>();
        }

        public async Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsyncByCollectionId(int collectionId)
        {
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

            var parameters = new DynamicParameters();
            parameters.Add("@CollectionId", collectionId);

            IEnumerable<OwnedCardPrintAggregate> foundCollections = null;
            try
            {
                using var dbConnection = new SQLiteConnection(SQLiteDatabaseCreator.GetConnectionString);
                foundCollections = await dbConnection.QueryAsync<OwnedCardPrintAggregate>(
                    sql: storedProcedure,
                    param: parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return foundCollections ?? Enumerable.Empty<OwnedCardPrintAggregate>();
        }
    }
}
