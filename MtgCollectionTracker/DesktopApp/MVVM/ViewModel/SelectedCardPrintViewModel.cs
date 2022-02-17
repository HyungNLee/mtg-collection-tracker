using DesktopApp.Event;
using DesktopApp.Event.EventModels;
using DesktopApp.MVVM.Model;

using Prism.Mvvm;

namespace DesktopApp.MVVM.ViewModel
{
    internal class SelectedCardPrintViewModel : BindableBase
    {
        /// <summary>
        /// The selected card to show.
        /// </summary>
        private CardPrint _selectedCardPrint;
        public CardPrint SelectedCardPrint
        {
            get { return _selectedCardPrint; }
            set { SetProperty(ref _selectedCardPrint, value); }
        }

        public SelectedCardPrintViewModel()
        {
            // Subscribe to CardPrintSelectedEvent
            ApplicationEventManager.Instance.Subscribe<CardPrintSelectedEvent>(args => SelectedCardPrint = args.SelectedCardPrint);
        }
    }
}
