using Serilog;

namespace DesktopApp.Event.EventModels
{
    /// <summary>
    /// Represents that a card operation was successful.
    /// </summary>
    internal class CardOperationSuccessEvent : IApplicationEvent
    {
        public CardOperationSuccessEvent()
        {
            Log.Debug($"{nameof(CardOperationSuccessEvent)}: Constructor");
        }
    }
}
