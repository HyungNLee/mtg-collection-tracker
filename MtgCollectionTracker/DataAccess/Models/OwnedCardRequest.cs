namespace DataAccess.Models
{
    public class OwnedCardRequest
    {
        public int CardPrintId { get; set; }
        public int CollectionId { get; set; }
        public bool IsFoil { get; set; }
    }
}
