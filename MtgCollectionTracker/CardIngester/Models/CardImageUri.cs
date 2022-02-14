using System.Text.Json.Serialization;

namespace CardIngester.Models
{
	internal class CardImageUri
	{
		[JsonPropertyName("normal")]
		public string Normal { get; set; }
	}
}
