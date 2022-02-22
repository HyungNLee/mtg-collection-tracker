using System.Collections.ObjectModel;
using System.Threading.Tasks;

using DataAccess.Services;
using DataAccess.Sqlite;

using DesktopApp.Event;
using DesktopApp.Event.EventModels;
using DesktopApp.MVVM.Model;

using Prism.Mvvm;

using Serilog;

namespace DesktopApp.MVVM.ViewModel
{
    internal class SelectedCardPrintViewModel : BindableBase
    {
        private readonly ICollectionService _collectionService;

        /// <summary>
        /// The selected card to show.
        /// </summary>
        private CardPrint _selectedCardPrint;
        public CardPrint SelectedCardPrint
        {
            get { return _selectedCardPrint; }
            set
            {
                SetProperty(ref _selectedCardPrint, value);
                var _ = GetOwnedCardsByCardAsync();
            }
        }

        /// <summary>
        /// A list of all prints of the owned card.
        /// </summary>
        private ObservableCollection<OwnedCardPrintAggregate> _ownedCards;
        public ObservableCollection<OwnedCardPrintAggregate> OwnedCards
        {
            get { return _ownedCards; }
            set { SetProperty(ref _ownedCards, value); }
        }

        public SelectedCardPrintViewModel()
        {
            Log.Debug($"{nameof(SelectedCardPrintViewModel)}: Constructor");

            _collectionService = new SQLiteCollectionService();

            OwnedCards = new ObservableCollection<OwnedCardPrintAggregate>();

            // Subscribe to CardPrintSelectedEvent
            ApplicationEventManager.Instance.Subscribe<CardPrintSelectedEvent>(args => SelectedCardPrint = args.SelectedCardPrint);

            ApplicationEventManager.Instance.Subscribe<CardOperationSuccessEvent>(async args =>
            {
                await GetOwnedCardsByCardAsync();
            });
        }

        /// <summary>
        /// Populates the owned cards collection of all owned cards of the selected card.
        /// </summary>
        /// <returns></returns>
        private async Task GetOwnedCardsByCardAsync()
        {
            Log.Debug($"{nameof(SelectedCardPrintViewModel)}: {nameof(GetOwnedCardsByCardAsync)}");

            if (SelectedCardPrint == null)
            {
                return;
            }

            var cards = await _collectionService.GetOwnedCardsAggregatesAsyncByCardId(SelectedCardPrint.CardId);

            OwnedCards.Clear();

            foreach (var card in cards)
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
    }
}
