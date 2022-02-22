using DesktopApp.MVVM.Model;

using Serilog;

namespace DesktopApp.Event.EventModels
{
    /// <summary>
    /// An event that triggers when a card is selected in a data grid view.
    /// </summary>
    internal class CardPrintSelectedEvent : IApplicationEvent
    {
        public CardPrint SelectedCardPrint { get; private set; }

        public CardPrintSelectedEvent(CardPrint selectedCardPrint)
        {
            Log.Debug($"{nameof(CardPrintSelectedEvent)}: Constructor");

            SelectedCardPrint = selectedCardPrint;
        }
    }
}
