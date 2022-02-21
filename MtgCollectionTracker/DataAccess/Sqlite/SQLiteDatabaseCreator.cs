using System.Data.SQLite;

namespace DataAccess.Sqlite
{
    internal static class SQLiteDatabaseCreator
    {
        private const string _dbName = "MtgCollection.db";
        private static readonly string _dbFilePath = $"{Path.Combine(Directory.GetCurrentDirectory(), _dbName)}";

        /// <summary>
        /// Returns the connection string to the SQLite database.
        /// </summary>
        public static string GetConnectionString = $"Data Source={_dbFilePath}";

        /// <summary>
        /// Creates the SQLite database if it doesn't exist.
        /// </summary>
        public static void CreateDatabaseIfNotExists()
        {
            if (!File.Exists(_dbFilePath))
            {
                SQLiteConnection.CreateFile(_dbFilePath);

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

        private static void CreateSetTable()
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

        private static void CreateCardPrintTable()
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

        private static void CreateCollectionTable()
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

        private static void CreateOwnedCardTable()
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

        private static void CreateCardPrintDetailsView()
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

        private static void CreateOwnedCardSumView()
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

        private static void InsertMainCollection()
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
    }
}
