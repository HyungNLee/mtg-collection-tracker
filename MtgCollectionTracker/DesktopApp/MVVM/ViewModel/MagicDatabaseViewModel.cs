﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using DataAccess.Services;

using DesktopApp.Event;
using DesktopApp.Event.EventModels;
using DesktopApp.MVVM.Model;

using Microsoft.Extensions.Options;

using Prism.Mvvm;

using DataAccessModels = DataAccess.Models;

namespace DesktopApp.MVVM.ViewModel
{
    internal class MagicDatabaseViewModel : BindableBase
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
            set
            {
                SetProperty(ref _selectedCardPrint, value);
                ApplicationEventManager.Instance.Publish(new CardPrintSelectedEvent(_selectedCardPrint));
            }
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
                FilterCardPrices();
            }
        }

        /// <summary>
        /// The filtered card print list.
        /// </summary>
        public ObservableCollection<CardPrint> FilteredCardPrints { get; private set; }

        public MagicDatabaseViewModel()
        {
            var dataAccessConfig = new DataAccessModels.DataAccessConfig
            {
                ConnectionString = @"Server=localhost;Database=MtgCollection;Trusted_Connection=True;"
            };
            var config = Options.Create(dataAccessConfig);
            _cardPrintService = new CardPrintService(config);

            CardPrints = new ObservableCollection<CardPrint>();
            FilteredCardPrints = new ObservableCollection<CardPrint>();

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

            // Need this here otherwise grid won't show data on initial load.
            FilterCardPrices();
        }

        /// <summary>
        /// Clears and adds card prints that match the filter into the FilteredCardPrints collection
        /// </summary>
        private void FilterCardPrices()
        {
            FilteredCardPrints.Clear();

            foreach (var cardPrint in _cardPrints)
            {
                if (CardPrintTextFilter(cardPrint))
                {
                    FilteredCardPrints.Add(cardPrint);
                }
            }

            RaisePropertyChanged(nameof(FilteredCardPrints));
        }

        private bool CardPrintTextFilter(CardPrint item)
        {
            if (string.IsNullOrEmpty(CardPrintTextSearch))
                return true;
            else
                return item.CardName.Contains(CardPrintTextSearch, StringComparison.OrdinalIgnoreCase);
        }
    }
}