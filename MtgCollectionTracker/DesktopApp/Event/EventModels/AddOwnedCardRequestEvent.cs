namespace DesktopApp.Event.EventModels
{
    /// <summary>
    /// An event that represents a request to add a new owned card.
    /// </summary>
    internal class AddOwnedCardRequestEvent : IApplicationEvent
    {
        public int CardPrintId { get; private set; }
        public bool IsFoil { get; private set; }

        public AddOwnedCardRequestEvent(int cardPrintId, bool isFoil = false)
        {
            CardPrintId = cardPrintId;
            IsFoil = isFoil;
        }
    }
}
