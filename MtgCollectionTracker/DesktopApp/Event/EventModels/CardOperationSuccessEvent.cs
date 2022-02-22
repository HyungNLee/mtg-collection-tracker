using Serilog;

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
        public int CardPrintId { get; private set; }
        public bool IsFoil { get; private set; }
        public int Count { get; private set; }
        public CardOperation OperationType { get; private set; }

        public CardOperationSuccessEvent(int cardPrintId, bool isFoil, int count, CardOperation operationType)
        {
            Log.Debug($"{nameof(CardOperationSuccessEvent)}: Constructor");

            CardPrintId = cardPrintId;
            IsFoil = isFoil;
            Count = count;
            OperationType = operationType;
        }
    }
}
