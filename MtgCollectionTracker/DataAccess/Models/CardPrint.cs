namespace DataAccess.Models
{
	public class CardPrint
	{
		public int Id { get; set; }
		public int CardId { get; set; }
		public int SetId { get; set; }
		public string PictureUrl { get; set; }
		public string FlipPictureUrl { get; set; }
	}
}
