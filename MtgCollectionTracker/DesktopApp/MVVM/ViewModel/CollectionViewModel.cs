using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using DataAccess.Services;

using DesktopApp.Event;
using DesktopApp.Event.EventModels;
using DesktopApp.MVVM.Model;

using Microsoft.Extensions.Options;

using Prism.Commands;
using Prism.Mvvm;

using DataAccessModels = DataAccess.Models;

namespace DesktopApp.MVVM.ViewModel
{
    internal class CollectionViewModel : BindableBase
    {
        private readonly ICollectionService _collectionService;

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
                LoadOwnedCardsByCollection();
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
                            }));
                }
            }
        }

        public CollectionViewModel()
        {
            var dataAccessConfig = new DataAccessModels.DataAccessConfig
            {
                ConnectionString = @"Server=localhost;Database=MtgCollection;Trusted_Connection=True;"
            };
            var config = Options.Create(dataAccessConfig);
            _collectionService = new CollectionService(config);

            Collections = new ObservableCollection<CardCollection>();
            OwnedCards = new ObservableCollection<OwnedCardPrintAggregate>();

            LoadCollectionsAsync();

            // Event subscribing
            ApplicationEventManager.Instance.Subscribe<AddOwnedCardRequestEvent>(args =>
            {
                if (SelectedCollection == null)
                {
                    return;
                }

                AddOwnedCardAsync(args.CardPrintId, SelectedCollection.Id, args.IsFoil);
            });
        }

        /// <summary>
        /// Used by the DataGrid keybind to add new cards.
        /// </summary>
        public ICommand AddOwnedCardCommand
        {
            get
            {
                var request = new DelegateCommand(() => AddCurrentlySelectedCard());
                return request;
            }
        }

        /// <summary>
        /// Loads all collections.
        /// </summary>
        /// <returns></returns>
        private async Task LoadCollectionsAsync()
        {
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
        private async Task LoadOwnedCardsByCollection()
        {
            if (SelectedCollection == null)
            {
                return;
            }

            OwnedCards.Clear();

            var ownedCards = await _collectionService.GetOwnedCardsAggregatesAsync(SelectedCollection.Id);

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

            RaisePropertyChanged(nameof(OwnedCards));
        }

        private async Task AddCurrentlySelectedCard()
        {
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
            var newRequest = new DataAccessModels.AddOwnedCardRequest
            {
                CardPrintId = cardPrintId,
                CollectionId = collectionId,
                IsFoil = isFoil
            };

            await _collectionService.AddOwnedCardAsync(newRequest);

            // If card already exists in the list, don't refresh the whole list. Just update the count property.
            var foundCard = OwnedCards.FirstOrDefault(card => card.CardPrintId == cardPrintId);
            if (foundCard != null)
            {
                foundCard.Count++;
                return;
            }
            await LoadOwnedCardsByCollection();
        }
    }
}
