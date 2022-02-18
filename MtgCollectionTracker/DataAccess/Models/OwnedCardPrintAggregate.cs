namespace DataAccess.Models
{
    public class OwnedCardPrintAggregate
    {
        public int CardId { get; set; }
        public string CardName { get; set; }
        public int CardPrintId { get; set; }
        public int SetId { get; set; }
        public string SetName { get; set; }
        public int CollectionId { get; set; }
        public string CollectionName { get; set; }
        public bool IsFoil { get; set; }
        public string PictureUrl { get; set; }
        public string FlipPictureUrl { get; set; }
        public int Count { get; set; }
    }
}
