using System.Text.Json;

using CardIngester.Models;

using DataAccessModels = DataAccess.Models;

using Microsoft.Extensions.Options;

namespace CardIngester
{
	internal class Program
    {
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            Console.WriteLine("Starting Scryfall MTG Card Ingester...");

            var dataAccessConfig = new DataAccessModels.DataAccessConfig
            {
                ConnectionString = @"Server=localhost;Database=MtgCollection;Trusted_Connection=True;"
            };
            var options = Options.Create(dataAccessConfig);
            var dbService = new DataAccess.Services.CardPrintService(options);
            var ingesterService = new CardIngesterService(dbService);

			Console.WriteLine("Please enter the full path to the Scryfall cards json file.");
            // This json file is usually a bulk card file from Scryfall
            // See: Files - Default Cards at https://scryfall.com/docs/api/bulk-data
            var cardJsonPath = Console.ReadLine();

            Console.WriteLine("Converting json to objects...");
            var foundJson = File.ReadAllText(cardJsonPath);

            var cardList = JsonSerializer.Deserialize<IEnumerable<Card>>(foundJson);

            foreach (var card in cardList)
            {
                Console.WriteLine($"Ingesting '{card.Name}' in set '{card.SetName}'...");
                await ingesterService.IngestCard(card);
                Console.WriteLine("Successfully ingested!");
            }

            Console.WriteLine("Finished ingesting all cards!");
            Console.ReadLine();
        }
    }
}
