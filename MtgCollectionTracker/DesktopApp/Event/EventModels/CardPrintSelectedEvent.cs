using DesktopApp.MVVM.Model;

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
            SelectedCardPrint = selectedCardPrint;
        }
    }
}
