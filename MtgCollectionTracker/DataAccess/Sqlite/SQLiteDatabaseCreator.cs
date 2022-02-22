using System.Data.SQLite;

using Serilog;

namespace DataAccess.Sqlite
{
    public static class SQLiteDatabaseCreator
    {
        /// <summary>
        /// Gets the database file name.
        /// </summary>
        public const string DatabaseName = "MtgCollection.db";

        /// <summary>
        /// Gets the full file path to the database file.
        /// </summary>
        public static readonly string DatabaseFilePath = $"{Path.Combine(Directory.GetCurrentDirectory(), DatabaseName)}";

        /// <summary>
        /// Returns the connection string to the SQLite database.
        /// </summary>
        internal static string GetConnectionString = $"Data Source={DatabaseFilePath}";

        /// <summary>
        /// Creates the SQLite database if it doesn't exist.
        /// </summary>
        internal static void CreateDatabaseIfNotExists()
        {
            Log.Debug($"{nameof(SQLiteDatabaseCreator)}: {nameof(CreateDatabaseIfNotExists)}");

            if (!File.Exists(DatabaseFilePath))
            {
                Log.Information($"Database not found. Creating database file.");

                SQLiteConnection.CreateFile(DatabaseFilePath);

                CreateCardTable();
                CreateSetTable();
                CreateCardPrintTable();
                CreateCollectionTable();
                CreateOwnedCardTable();
                CreateCardPrintDetailsView();
                CreateOwnedCardSumView();

                InsertMainCollection();
            }
        }

        private static void CreateCardTable()
        {
            Log.Debug($"{nameof(SQLiteDatabaseCreator)}: {nameof(CreateCardTable)}");

            try
            {
                using var db = new SQLiteConnection(GetConnectionString);
                db.Open();
                string sql = @"
                        CREATE TABLE [Card] (
                            Id    INTEGER NOT NULL UNIQUE,
                            Name  TEXT NOT NULL UNIQUE,
                            PRIMARY KEY(Id AUTOINCREMENT)
                        );";

                SQLiteCommand command = new(sql, db);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{ nameof(SQLiteDatabaseCreator)}: { nameof(CreateCardTable)}");
                throw;
            }
        }

        private static void CreateSetTable()
        {
            Log.Debug($"{nameof(SQLiteDatabaseCreator)}: {nameof(CreateSetTable)}");

            try
            {
                using var db = new SQLiteConnection(GetConnectionString);
                db.Open();
                string sql = @"
                        CREATE TABLE [Set] (
                            Id    INTEGER NOT NULL UNIQUE,
                            Name  TEXT NOT NULL UNIQUE,
                            PRIMARY KEY(Id AUTOINCREMENT)
                        );";

                SQLiteCommand command = new(sql, db);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{ nameof(SQLiteDatabaseCreator)}: { nameof(CreateSetTable)}");
                throw;
            }
        }

        private static void CreateCardPrintTable()
        {
            Log.Debug($"{nameof(SQLiteDatabaseCreator)}: {nameof(CreateCardPrintTable)}");

            try
            {
                using var db = new SQLiteConnection(GetConnectionString);
                db.Open();
                string sql = @"
                        CREATE TABLE CardPrint (
                            Id	INTEGER NOT NULL UNIQUE,
                            CardId	INTEGER NOT NULL,
                            SetId	INTEGER NOT NULL,
                            PictureUrl	TEXT,
                            FlipPictureUrl	TEXT,
                            UNIQUE(CardId,SetId),
                            FOREIGN KEY(SetId) REFERENCES [Set](Id),
                            FOREIGN KEY(CardId) REFERENCES Card(Id),
                            PRIMARY KEY(Id AUTOINCREMENT)
                        );";

                SQLiteCommand command = new(sql, db);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{ nameof(SQLiteDatabaseCreator)}: { nameof(CreateCardPrintTable)}");
                throw;
            }
        }

        private static void CreateCollectionTable()
        {
            Log.Debug($"{nameof(SQLiteDatabaseCreator)}: {nameof(CreateCollectionTable)}");

            try
            {
                using var db = new SQLiteConnection(GetConnectionString);
                db.Open();
                string sql = @"
                        CREATE TABLE [Collection] (
                            Id	INTEGER NOT NULL UNIQUE,
                            Name	TEXT NOT NULL UNIQUE,
                            IsDeck	INTEGER NOT NULL DEFAULT 0 CHECK(IsDeck = 0 OR IsDeck = 1),
                            MainboardId	INTEGER,
                            SideboardId	INTEGER,
                            CHECK((IsDeck = 0 AND MainboardId IS null AND SideboardId IS null) OR (IsDeck = 1 AND ((MainboardId IS null AND SideboardId IS null) OR (MainboardId IS NOT null AND SideboardId IS null) OR (MainboardId IS null AND SideboardId IS NOT null)))),
                            PRIMARY KEY(Id AUTOINCREMENT)
                        );";

                SQLiteCommand command = new(sql, db);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{ nameof(SQLiteDatabaseCreator)}: { nameof(CreateCollectionTable)}");
                throw;
            }
        }

        private static void CreateOwnedCardTable()
        {
            Log.Debug($"{nameof(SQLiteDatabaseCreator)}: {nameof(CreateOwnedCardTable)}");

            try
            {
                using var db = new SQLiteConnection(GetConnectionString);
                db.Open();
                string sql = @"
                        CREATE TABLE OwnedCard (
                            Id	INTEGER NOT NULL UNIQUE,
                            CardPrintId	INTEGER NOT NULL,
                            CollectionId	INTEGER NOT NULL,
                            IsFoil	INTEGER NOT NULL DEFAULT 0 CHECK(IsFoil = 0 OR IsFoil = 1),
                            PRIMARY KEY(Id AUTOINCREMENT),
                            FOREIGN KEY(CardPrintId) REFERENCES CardPrint(Id),
                            FOREIGN KEY(CollectionId) REFERENCES [Collection](Id)
                        );";

                SQLiteCommand command = new(sql, db);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{ nameof(SQLiteDatabaseCreator)}: { nameof(CreateOwnedCardTable)}");
                throw;
            }
        }

        private static void CreateCardPrintDetailsView()
        {
            Log.Debug($"{nameof(SQLiteDatabaseCreator)}: {nameof(CreateCardPrintDetailsView)}");

            try
            {
                using var db = new SQLiteConnection(GetConnectionString);
                db.Open();
                string sql = @"        
                    CREATE VIEW vw_CardPrintDetails
                    as
                        select
                            cp.Id,
                            cp.CardId,
                            c.[Name] as CardName,
                            cp.SetId,
                            s.[Name] as SetName,
                            cp.PictureUrl,
                            cp.FlipPictureUrl
                        from CardPrint as cp
                        inner join [Card] as c on cp.CardId = c.Id
                        inner join [Set] as s on cp.SetId = s.Id";

                SQLiteCommand command = new(sql, db);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{ nameof(SQLiteDatabaseCreator)}: { nameof(CreateCardPrintDetailsView)}");
                throw;
            }
        }

        private static void CreateOwnedCardSumView()
        {
            Log.Debug($"{nameof(SQLiteDatabaseCreator)}: {nameof(CreateOwnedCardSumView)}");

            try
            {
                using var db = new SQLiteConnection(GetConnectionString);
                db.Open();
                string sql = @"        
                    CREATE VIEW vw_OwnedCardSum
                    as
                        select
                            CardPrintId,
                            CollectionId,
                            IsFoil,
                            Count(*) as [Count]
                        from OwnedCard
                        group by
                            CardPrintId,
                            CollectionId,
                            IsFoil;";

                SQLiteCommand command = new(sql, db);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{ nameof(SQLiteDatabaseCreator)}: { nameof(CreateOwnedCardSumView)}");
                throw;
            }
        }

        private static void InsertMainCollection()
        {
            Log.Debug($"{nameof(SQLiteDatabaseCreator)}: {nameof(InsertMainCollection)}");

            try
            {
                using var db = new SQLiteConnection(GetConnectionString);
                db.Open();
                string sql = @"        
                    insert into [Collection] (
                        [Name],
                        IsDeck
                    )
                    values (
                        'Main Collection',
                        0
                    );";

                SQLiteCommand command = new(sql, db);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{ nameof(SQLiteDatabaseCreator)}: { nameof(InsertMainCollection)}");
                throw;
            }
        }
    }
}
