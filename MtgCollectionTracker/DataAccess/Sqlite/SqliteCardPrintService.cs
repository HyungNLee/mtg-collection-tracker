using System.Data.SQLite;

using DataAccess.Models;
using DataAccess.Services;

namespace DataAccess.Sqlite
{
    public class SqliteCardPrintService : ICardPrintService
    {
        private const string _dbName = "MtgCollection.db";

        private string _dbFilePath = $"Filename={Path.Combine(Directory.GetCurrentDirectory(), _dbName)}";

        public SqliteCardPrintService()
        {
            SQLiteDatabaseCreator.CreateDatabaseIfNotExists();
        }

        public Task<Card> GetCardAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<CardPrintDetail> GetCardPrintDetailAsync(int cardId, int setId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CardPrintDetail>> GetCardPrintDetailsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Set> GetSetAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertCardAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertCardPrintAsync(int cardId, int setId, string pictureUrl, string flipPictureUrl)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertSetAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
