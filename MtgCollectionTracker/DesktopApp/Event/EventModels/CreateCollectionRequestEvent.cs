namespace DesktopApp.Event.EventModels
{
    internal class CreateCollectionRequestEvent : IApplicationEvent
    {
        public string Name { get; private set; }
        public bool IsDeck { get; private set; }

        public CreateCollectionRequestEvent(string name, bool isDeck)
        {
            Name = name;
            IsDeck = isDeck;
        }
    }
}
