using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using DataAccess.Services;
using DataAccess.Sqlite;

using DesktopApp.Event;
using DesktopApp.Event.EventModels;
using DesktopApp.MVVM.Model;
using DesktopApp.MVVM.View;

using Prism.Commands;
using Prism.Mvvm;

using Serilog;

using DataAccessModels = DataAccess.Models;

namespace DesktopApp.MVVM.ViewModel
{
    internal class CollectionViewModel : BindableBase
    {
        private readonly ICollectionService _collectionService;
        private readonly ICardPrintService _cardPrintService;

        /// <summary>
        /// A list of all collections.
        /// </summary>
        private ObservableCollection<CardCollection> _collections;
        public ObservableCollection<CardCollection> Collections
        {
            get { return _collections; }
            set { SetProperty(ref _collections, value); }
        }

        /// <summary>
        /// Currently selected card collection.
        /// </summary>
        private CardCollection _selectedCollection;
        public CardCollection SelectedCollection
        {
            get { return _selectedCollection; }
            set
            {
                SetProperty(ref _selectedCollection, value);
                var _ = LoadOwnedCardsByCollectionAsync();
            }
        }

        /// <summary>
        /// A collection of owned card aggregates.
        /// </summary>
        private ObservableCollection<OwnedCardPrintAggregate> _ownedCards;
        public ObservableCollection<OwnedCardPrintAggregate> OwnedCards
        {
            get { return _ownedCards; }
            set { SetProperty(ref _ownedCards, value); }
        }

        /// <summary>
        /// Currently selected owned card.
        /// </summary>
        private OwnedCardPrintAggregate _selectedOwnedCard;
        public OwnedCardPrintAggregate SelectedOwnedCard
        {
            get { return _selectedOwnedCard; }
            set
            {
                SetProperty(ref _selectedOwnedCard, value);

                if (value != null)
                {
                    ApplicationEventManager.Instance.Publish(
                        new CardPrintSelectedEvent(
                            new CardPrint
                            {
                                BackPictureUrl = _selectedOwnedCard.BackPictureUrl,
                                CardName = _selectedOwnedCard.CardName,
                                FrontPictureUrl = _selectedOwnedCard.FrontPictureUrl,
                                Id = _selectedOwnedCard.CardPrintId,
                                SetName = _selectedOwnedCard.SetName,
                                SetId = _selectedOwnedCard.SetId,
                                CardId  = _selectedOwnedCard.CardId,
                            }));
                }
            }
        }

        /// <summary>
        /// The current text in the text search box.
        /// </summary>
        private string _cardTextSearch;
        public string CardTextSearch
        {
            get { return _cardTextSearch; }
            set { SetProperty(ref _cardTextSearch, value); }
        }

        /// <summary>
        /// The filtered card print list.
        /// </summary>
        public ObservableCollection<OwnedCardPrintAggregate> FilteredOwnedCards { get; private set; }

        /// <summary>
        /// The sum of all owned cards in the filter.
        /// </summary>
        public int FilteredOwnedCardsSum => FilteredOwnedCards.Sum(card => card.Count);

        /// <summary>
        /// Command to create a new sideboard.
        /// </summary>
        public DelegateCommand AddSideboardCommand { get; set; }

        /// <summary>
        /// Command to open up the AddCollectionDialogWindow.
        /// </summary>
        public DelegateCommand ShowAddCollectionDialogCommand { get; private set; }

        /// <summary>
        /// Command to show the DeleteCollectionDialogWindow.
        /// </summary>
        public DelegateCommand ShowDeleteCollectionDialogCommand { get; private set; }

        public CollectionViewModel()
        {
            Log.Debug($"{nameof(CollectionViewModel)}: Constructor");

            _collectionService = new SQLiteCollectionService();
            _cardPrintService = new SQLiteCardPrintService();

            Collections = new ObservableCollection<CardCollection>();
            OwnedCards = new ObservableCollection<OwnedCardPrintAggregate>();
            FilteredOwnedCards = new ObservableCollection<OwnedCardPrintAggregate>();

            AddSideboardCommand = new DelegateCommand(async () => await AddSideboardAsync());
            ShowAddCollectionDialogCommand = new DelegateCommand(() => ShowAddCollectionDialog());
            ShowDeleteCollectionDialogCommand = new DelegateCommand(() => ShowDeleteCollectionDialog());

            var _ = LoadCollectionsAsync();

            // ** Event subscribing **
            // Add new card event is received.
            ApplicationEventManager.Instance.Subscribe<AddOwnedCardRequestEvent>(async args =>
            {
                if (SelectedCollection == null)
                {
                    return;
                }

                await AddOwnedCardAsync(args.CardPrintId, SelectedCollection.Id, args.IsFoil);
            });

            // Create new non-sideboard collection event is received.
            ApplicationEventManager.Instance.Subscribe<CreateCollectionRequestEvent>(async args =>
            {
                await AddCollectionAsync(args.Name, args.IsDeck);
            });
        }

        /// <summary>
        /// Used by the DataGrid keybind to add new cards.
        /// </summary>
        public ICommand AddOwnedCardCommand
        {
            get
            {
                Log.Debug($"{nameof(CollectionViewModel)}: {nameof(AddOwnedCardCommand)}");

                var request = new DelegateCommand(async () => await AddCurrentlySelectedCardAsync());
                return request;
            }
        }

        /// <summary>
        /// Used by the DataGrid to delete a single instance of selected owned card.
        /// </summary>
        public ICommand DeleteSingleSelectedOwnedCard
        {
            get
            {
                Log.Debug($"{nameof(CollectionViewModel)}: {nameof(DeleteSingleSelectedOwnedCard)}");

                var request = new DelegateCommand(async () => await DeleteSelectedOwnedCardAsync());
                return request;
            }
        }

        /// <summary>
        /// Used by the DataGrid to delete a single instance of selected owned card.
        /// </summary>
        public ICommand DeleteAllSelectedOwnedCard
        {
            get
            {
                Log.Debug($"{nameof(CollectionViewModel)}: {nameof(DeleteAllSelectedOwnedCard)}");

                var request = new DelegateCommand(async () => await DeleteSelectedOwnedCardAsync(true));
                return request;
            }
        }

        /// <summary>
        /// Used by the card text search to initiate filter on "Enter" key.
        /// </summary>
        public ICommand CardFilterCommand
        {
            get
            {
                Log.Debug($"{nameof(CollectionViewModel)}: {nameof(CardFilterCommand)}");

                return new DelegateCommand(() => FilterOwnedCards());
            }
        }

        /// <summary>
        /// Creates a new sideboard for a deck.
        /// </summary>
        public async Task AddSideboardAsync()
        {
            Log.Debug($"{nameof(CollectionViewModel)}: {nameof(AddSideboardAsync)}");

            if (SelectedCollection == null || !SelectedCollection.IsDeck)
            {
                Log.Information("Collection must be a deck to create a sideboard.");
                return;
            }

            var sideboardId = await _collectionService.AddDeckSideboardAsync(SelectedCollection.Id);

            // Refresh Collections for now - may want to manually add just the sideboard to collections later.
            await LoadCollectionsAsync();
        }

        /// <summary>
        /// Loads all collections.
        /// </summary>
        /// <returns></returns>
        private async Task LoadCollectionsAsync()
        {
            Log.Debug($"{nameof(CollectionViewModel)}: {nameof(LoadCollectionsAsync)}");

            Collections.Clear();

            var collections = await _collectionService.GetCollectionsAsync();

            foreach (var collection in collections)
            {
                var newCollection = new CardCollection
                {
                    Id = collection.Id,
                    IsDeck = collection.IsDeck,
                    MainboardId = collection.MainboardId,
                    Name = collection.Name,
                    SideboardId = collection.SideboardId
                };

                Collections.Add(newCollection);
            }
        }

        /// <summary>
        /// Loads the owned cards by collection.
        /// </summary>
        /// <returns></returns>
        private async Task LoadOwnedCardsByCollectionAsync()
        {
            Log.Debug($"{nameof(CollectionViewModel)}: {nameof(LoadOwnedCardsByCollectionAsync)}");

            if (SelectedCollection == null)
            {
                return;
            }

            OwnedCards.Clear();

            var ownedCards = await _collectionService.GetOwnedCardsAggregatesAsyncByCollectionId(SelectedCollection.Id);

            foreach (var card in ownedCards)
            {
                var newOwnedCard = new OwnedCardPrintAggregate
                {
                    BackPictureUrl = card.FlipPictureUrl,
                    CardId = card.CardId,
                    CardName = card.CardName,
                    CardPrintId = card.CardPrintId,
                    CollectionId = card.CollectionId,
                    CollectionName = card.CollectionName,
                    FrontPictureUrl = card.PictureUrl,
                    IsFoil = card.IsFoil,
                    SetId = card.SetId,
                    SetName = card.SetName,
                    Count = card.Count
                };

                OwnedCards.Add(newOwnedCard);
            }


            // Need this here otherwise grid won't show data on initial load.
            FilterOwnedCards();
        }

        /// <summary>
        /// Adds an instance of the currently selected owned card.
        /// </summary>
        /// <returns></returns>
        private async Task AddCurrentlySelectedCardAsync()
        {
            Log.Debug($"{nameof(CollectionViewModel)}: {nameof(AddCurrentlySelectedCardAsync)}");

            if (SelectedOwnedCard == null)
            {
                return;
            }

            await AddOwnedCardAsync(SelectedOwnedCard.CardPrintId, SelectedCollection.Id, SelectedOwnedCard.IsFoil);
        }

        /// <summary>
        /// Adds a new non-foil version of a card.
        /// </summary>
        /// <returns></returns>
        private async Task AddOwnedCardAsync(int cardPrintId, int collectionId, bool isFoil = false)
        {
            Log.Debug($"{nameof(CollectionViewModel)}: {nameof(AddOwnedCardAsync)}");

            var newRequest = new DataAccessModels.OwnedCardRequest
            {
                CardPrintId = cardPrintId,
                CollectionId = collectionId,
                IsFoil = isFoil
            };

            await _collectionService.AddOwnedCardAsync(newRequest);

            var cardOperationSuccessEvent = new CardOperationSuccessEvent();
            ApplicationEventManager.Instance.Publish(cardOperationSuccessEvent);

            // If card already exists in the list, don't refresh the whole list. Just update the count property.
            var foundCard = OwnedCards.FirstOrDefault(card => card.CardPrintId == cardPrintId && card.IsFoil == isFoil);
            if (foundCard != null)
            {
                foundCard.Count++;
                RaisePropertyChanged(nameof(FilteredOwnedCardsSum));
                return;
            }

            await LoadOwnedCardsByCollectionAsync();
        }

        /// <summary>
        /// Deletes either a single or all of the currently selected owned card.
        /// </summary>
        /// <param name="numberToDelete"></param>
        /// <returns></returns>
        private async Task DeleteSelectedOwnedCardAsync(bool deleteAll = false)
        {
            Log.Debug($"{nameof(CollectionViewModel)}: {nameof(DeleteSelectedOwnedCardAsync)}");

            if (SelectedOwnedCard == null)
            {
                Log.Debug($"{nameof(CollectionViewModel)}: {nameof(DeleteSelectedOwnedCardAsync)} - {nameof(SelectedOwnedCard)} is null");
                return;
            }

            var request = new DataAccessModels.OwnedCardRequest
            {
                CardPrintId = SelectedOwnedCard.CardPrintId,
                CollectionId = SelectedCollection.Id,
                IsFoil = SelectedOwnedCard.IsFoil
            };

            var numberToDelete = deleteAll ? SelectedOwnedCard.Count : 1;

            await _collectionService.DeleteOwnedCardsAsync(request, numberToDelete);

            var cardOperationSuccessEvent = new CardOperationSuccessEvent();
            ApplicationEventManager.Instance.Publish(cardOperationSuccessEvent);

            if (deleteAll || SelectedOwnedCard.Count == 1)
            {
                OwnedCards.Remove(SelectedOwnedCard);
                FilteredOwnedCards.Remove(SelectedOwnedCard);
                SelectedOwnedCard = null;
                return;
            }

            SelectedOwnedCard.Count--;
        }

        /// <summary>
        /// Adds a new non-sideboard collection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isDeck"></param>
        /// <returns></returns>
        private async Task AddCollectionAsync(string name, bool isDeck)
        {
            Log.Debug($"{nameof(CollectionViewModel)}: {nameof(AddCollectionAsync)}");

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var request = new DataAccessModels.AddCollectionRequest
            {
                IsDeck = isDeck,
                Name = name,
            };

            var id = await _collectionService.AddCollectionAsync(request);

            var newCollection = new CardCollection
            {
                Id = id,
                IsDeck = isDeck,
                Name = name
            };

            Collections.Add(newCollection);
        }

        /// <summary>
        /// Clears and adds card prints that match the filter into the FilteredOwnedCards collection
        /// </summary>
        private void FilterOwnedCards()
        {
            Log.Debug($"{nameof(CollectionViewModel)}: {nameof(FilterOwnedCards)}");

            FilteredOwnedCards.Clear();

            foreach (var cardPrint in OwnedCards)
            {
                if (CardTextFilter(cardPrint))
                {
                    FilteredOwnedCards.Add(cardPrint);
                }
            }

            RaisePropertyChanged(nameof(FilteredOwnedCards));
            RaisePropertyChanged(nameof(FilteredOwnedCardsSum));
        }

        private bool CardTextFilter(OwnedCardPrintAggregate item)
        {
            if (string.IsNullOrEmpty(CardTextSearch))
                return true;
            else
                return item.CardName.Contains(CardTextSearch, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Opens up the "AddCollectionDialogWindow"
        /// </summary>
        private void ShowAddCollectionDialog()
        {
            Log.Debug($"{nameof(CollectionViewModel)}: {nameof(ShowAddCollectionDialog)}");

            var dialogWindow = new AddCollectionDialogWindow();

            var result = dialogWindow.ShowDialog();
        }

        /// <summary>
        /// Opens up a confirmation dialog window to delete a collection.
        /// </summary>
        private async void ShowDeleteCollectionDialog()
        {
            Log.Debug($"{nameof(CollectionViewModel)}: {nameof(ShowDeleteCollectionDialog)}");

            if (SelectedCollection == null)
            {
                return;
            }

            if (SelectedCollection.Id == 1)
            {
                // Selected Collection is the main collection
                MessageBox.Show(
                    $"The {SelectedCollection.Name} is the default collection and can't be deleted.",
                    "Collection", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            string message = $"Are you sure you want to delete {SelectedCollection.Name}?\nAny cards in this collection/deck will be moved to the main collection.";

            if (MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
            {
                await _collectionService.RemoveCollection(SelectedCollection.Id);

                await RefreshAllDataAsync();
            }
        }

        /// <summary>
        /// Clears and refreshs all data and datagrids.
        /// </summary>
        /// <returns></returns>
        private async Task RefreshAllDataAsync()
        {
            SelectedCollection = null;
            SelectedOwnedCard = null;
            await LoadCollectionsAsync();
            OwnedCards.Clear();
            FilterOwnedCards();

            // Refreshes the selected card datagrid.
            ApplicationEventManager.Instance.Publish(new CardOperationSuccessEvent());
        }
    }
}
