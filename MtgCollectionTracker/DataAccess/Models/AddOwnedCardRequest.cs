namespace DataAccess.Models
{
    public class AddOwnedCardRequest
    {
        public int CardPrintId { get; set; }
        public int CollectionId { get; set; }
        public bool IsFoil { get; set; }
    }
}
