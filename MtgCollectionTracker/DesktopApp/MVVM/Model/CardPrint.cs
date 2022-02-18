namespace DesktopApp.MVVM.Model
{
    internal class CardPrint
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string CardName { get; set; }
        public int SetId { get; set; }
        public string SetName { get; set; }
        public string FrontPictureUrl { get; set; }
        public string BackPictureUrl { get; set; }
    }
}
