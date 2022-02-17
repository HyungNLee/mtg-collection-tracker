namespace DataAccess.Models
{
    public class CardPrintDetail
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string CardName { get; set; }
        public int SetId { get; set; }
        public string SetName { get; set; }
        public string PictureUrl { get; set; }
        public string FlipPictureUrl { get; set; }
    }
}
