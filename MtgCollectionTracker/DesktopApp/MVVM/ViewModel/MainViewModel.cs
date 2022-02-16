using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;

using DataAccess.Services;

using DesktopApp.MVVM.Model;

using Microsoft.Extensions.Options;

using Prism.Mvvm;

using DataAccessModels = DataAccess.Models;

namespace DesktopApp.MVVM.ViewModel
{
    internal class MainViewModel : BindableBase
    {
        private readonly ICardPrintService _cardPrintService;

        /// <summary>
        /// A list of all card prints.
        /// </summary>
        private ObservableCollection<CardPrint> _cardPrints;
        public ObservableCollection<CardPrint> CardPrints
        {
            get { return _cardPrints; }
            set { SetProperty(ref _cardPrints, value); }
        }

        /// <summary>
        /// The selected card from the data grid view.
        /// </summary>
        private CardPrint _selectedCardPrint;
        public CardPrint SelectedCardPrint
        {
            get { return _selectedCardPrint; }
            set { SetProperty(ref _selectedCardPrint, value); }
        }

        /// <summary>
        /// The current text in the Card Print search box.
        /// </summary>
        private string _cardPrintTextSearch;
        public string CardPrintTextSearch
        {
            get { return _cardPrintTextSearch; }
            set
            {
                SetProperty(ref _cardPrintTextSearch, value);

                if (FilteredCardPrintsView != null)
                {
                    FilteredCardPrintsView.Refresh();
                }
            }
        }

        /// <summary>
        /// The filtered card print list.
        /// </summary>
        private ICollectionView _filteredCardPrintsView;
        public ICollectionView FilteredCardPrintsView
        {
            get { return _filteredCardPrintsView;}
        }

        public MainViewModel()
        {
            var dataAccessConfig = new DataAccessModels.DataAccessConfig
            {
                ConnectionString = @"Server=localhost;Database=MtgCollection;Trusted_Connection=True;"
            };
            var config = Options.Create(dataAccessConfig);
            _cardPrintService = new CardPrintService(config);

            CardPrints = new ObservableCollection<CardPrint>();
            _filteredCardPrintsView = new ListCollectionView(CardPrints)
            {
                Filter = new Predicate<object>(CardPrintTextFilter)
            };

            LoadCardPrintsAsync();
        }

        /// <summary>
        /// Loads all card prints for the Magic Database Data Grid.
        /// </summary>
        /// <returns></returns>
        public async Task LoadCardPrintsAsync()
        {
            var allCardPrints = await _cardPrintService.GetCardPrintDetailsAsync();

            foreach (var cardPrint in allCardPrints)
            {
                var cardModel = new CardPrint
                {
                    BackPictureUrl = cardPrint.FlipPictureUrl,
                    CardName = cardPrint.CardName,
                    FrontPictureUrl = cardPrint.PictureUrl,
                    Id = cardPrint.Id,
                    SetName = cardPrint.SetName
                };

                CardPrints.Add(cardModel);
            }
        }

        private bool CardPrintTextFilter(object item)
        {
            if (string.IsNullOrEmpty(CardPrintTextSearch))
                return true;
            else
                return (item as CardPrint).CardName.Contains(CardPrintTextSearch, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Not sure why I need this but the ListCollectionView filtering does not work if this isn't here.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
