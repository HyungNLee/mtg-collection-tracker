namespace DesktopApp.Event.EventModels
{
    internal enum CardOperation
    {
        Add = 0,
        Delete = 1
    }

    /// <summary>
    /// Represents a successful add or delete card operation.
    /// </summary>
    internal class CardOperationSuccessEvent : IApplicationEvent
    {
        public int CardPrintId { get; set; }
        public int Count { get; set; }
        public CardOperation OperationType { get; set; }

        public CardOperationSuccessEvent(int cardPrintId, int count, CardOperation operationType)
        {
            CardPrintId = cardPrintId;
            Count = count;
            OperationType = operationType;
        }
    }
}
