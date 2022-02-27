using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using DataAccess.Services;
using DataAccess.Sqlite;

using Prism.Commands;
using Prism.Mvvm;

using Serilog;

using DataAccessModels = DataAccess.Models;

namespace DesktopApp.MVVM.ViewModel
{
    internal class MainMenuBarViewModel : BindableBase
    {
        private readonly ICollectionService _collectionService;
        private readonly ICardPrintService _cardPrintService;

        /// <summary>
        /// Command to import owned cards.
        /// </summary>
        public DelegateCommand ImportCardCommand { get; private set; }

        /// <summary>
        /// Command export owned cards.
        /// </summary>
        public DelegateCommand ExportCardCommand { get; private set; }

        public MainMenuBarViewModel()
            : base()
        {
            Log.Debug($"{nameof(MainMenuBarViewModel)}: Constructor");

            _collectionService = new SQLiteCollectionService();
            _cardPrintService = new SQLiteCardPrintService();

            ImportCardCommand = new DelegateCommand(async () => await ImportOwnedCardsJsonAsync());
            ExportCardCommand = new DelegateCommand(async () => await ExportOwnedCardsJsonAsync());
        }

        private async Task ExportOwnedCardsJsonAsync()
        {
            Log.Debug($"{nameof(MainMenuBarViewModel)}: {nameof(ExportOwnedCardsJsonAsync)}");

            try
            {
                var ownedCards = await _collectionService.GetOwnedCardsExportFormatAsync();
                var jsonSerialized = JsonSerializer.Serialize(ownedCards);

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "OwnedCardsExport.json");
                File.WriteAllText(filePath, jsonSerialized);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(MainMenuBarViewModel)}: {nameof(ExportOwnedCardsJsonAsync)}");
                throw;
            }
        }

        private async Task ImportOwnedCardsJsonAsync()
        {
            Log.Debug($"{nameof(MainMenuBarViewModel)}: {nameof(ImportOwnedCardsJsonAsync)}");

            try
            {
                // Create backup of database first.
                var backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "backups");
                Directory.CreateDirectory(backupFolderPath);

                // Copy database
                var backupDbPath = Path.Combine(backupFolderPath, SQLiteDatabaseCreator.DatabaseName);
                File.Copy(SQLiteDatabaseCreator.DatabaseFilePath, backupDbPath, true);

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "OwnedCardsExport.json");

                var jsonText = File.ReadAllText(filePath);
                var ownedCards = JsonSerializer.Deserialize<IEnumerable<DataAccessModels.OwnedCardExport>>(jsonText)
                    ?? Enumerable.Empty<DataAccessModels.OwnedCardExport>();

                string sideboardIdentifier = " - Sideboard";
                foreach (var ownedCard in ownedCards)
                {
                    var foundCollection = await _collectionService.GetCollectionAsync(ownedCard.CollectionName);
                    if (foundCollection == null)
                    {
                        // Check if sideboard
                        if (ownedCard.CollectionName.Contains(sideboardIdentifier))
                        {
                            var cutoffIndex = ownedCard.CollectionName.LastIndexOf(sideboardIdentifier);
                            var mainboardName = ownedCard.CollectionName.Substring(0, cutoffIndex);
                            var foundMainboardCollection = await _collectionService.GetCollectionAsync(mainboardName);

                            if (foundMainboardCollection == null)
                            {
                                var collectionRequest = new DataAccessModels.AddCollectionRequest
                                {
                                    IsDeck = true,
                                    Name = mainboardName
                                };
                                await _collectionService.AddCollectionAsync(collectionRequest);
                                foundMainboardCollection = await _collectionService.GetCollectionAsync(mainboardName);
                            }

                            var sideboardId = await _collectionService.AddDeckSideboardAsync(foundMainboardCollection.Id);
                        }
                        else
                        {
                            var collectionRequest = new DataAccessModels.AddCollectionRequest
                            {
                                IsDeck = ownedCard.IsDeck,
                                Name = ownedCard.CollectionName
                            };
                            await _collectionService.AddCollectionAsync(collectionRequest);
                        }

                        foundCollection = await _collectionService.GetCollectionAsync(ownedCard.CollectionName);
                    }

                    var cardPrintDetails = await _cardPrintService.GetCardPrintDetailAsync(ownedCard.CardName, ownedCard.SetName);

                    for (int i = 0; i < ownedCard.Count; i++)
                    {
                        var ownedCardRequest = new DataAccessModels.OwnedCardRequest
                        {
                            CardPrintId = cardPrintDetails.Id,
                            CollectionId = foundCollection.Id,
                            IsFoil = ownedCard.IsFoil
                        };

                        await _collectionService.AddOwnedCardAsync(ownedCardRequest);
                    }
                }

                // TODO: Create event to refresh collection.
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(MainMenuBarViewModel)}: {nameof(ImportOwnedCardsJsonAsync)}");
                throw;
            }
        }
    }
}
