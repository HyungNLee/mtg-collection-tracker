namespace DesktopApp.MVVM.Model
{
    internal class OwnedCardPrintAggregate
    {
        public int CardId { get; set; }
        public string CardName { get; set; }
        public int CardPrintId { get; set; }
        public int SetId { get; set; }
        public string SetName { get; set; }
        public int CollectionId { get; set; }
        public string CollectionName { get; set; }
        public bool IsFoil { get; set; }
        public string FrontPictureUrl { get; set; }
        public string BackPictureUrl { get; set; }
        public int Count { get; set; }
    }
}
