namespace DataAccess.Models
{
    public class TransferCardRequest
    {
        public int CardPrintId { get; set; }
        public int SourceCollectionId { get; set; }
        public int DestinationCollectionId { get; set; }
        public bool IsFoil { get; set; }
        public int Count { get; set; }
    }
}
