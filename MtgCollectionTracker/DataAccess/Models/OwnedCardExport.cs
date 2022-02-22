namespace DataAccess.Models
{
    public class OwnedCardExport
    {
        public string CardName { get; set; }
        public string SetName { get; set; }
        public string CollectionName { get; set; }
        public bool IsDeck { get; set; }
        public bool IsFoil { get; set; }
        public int Count { get; set; }
    }
}
