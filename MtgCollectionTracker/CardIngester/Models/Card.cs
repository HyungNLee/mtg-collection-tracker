using System.Text.Json.Serialization;

namespace CardIngester.Models
{
	internal class Card
	{

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("image_uris")]
		public CardImageUri ImageUri { get; set; }

		[JsonPropertyName("set_name")]
		public string SetName { get; set; }

		[JsonPropertyName("card_faces")]
		public List<Card> CardFaces { get; set; }
	}
}
