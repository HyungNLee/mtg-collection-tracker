using CardIngester.Models;

using DataAccess.Services;

namespace CardIngester
{
    internal class CardIngesterService
    {
        private readonly ICardPrintService _cardPrintService;

        public CardIngesterService(ICardPrintService cardPrintService)
        {
            _cardPrintService = cardPrintService ?? throw new ArgumentNullException(nameof(cardPrintService));
        }

        public async Task IngestCard(Card card)
        {
            var foundCard = await _cardPrintService.GetCardAsync(card.Name);

            // Insert card if not found
            var cardId = 0;
            if (foundCard == null)
            {
                Console.WriteLine($"Adding new card '{card.Name}'...");

                cardId = await _cardPrintService.InsertCardAsync(card.Name);
            }
            else
            {
                Console.WriteLine($"Card '{card.Name}' already exists!");

                cardId = foundCard.Id;
            }

            // Insert set if not found
            var foundSet = await _cardPrintService.GetSetAsync(card.SetName);
            var setId = 0;
            if (foundSet == null)
            {
                Console.WriteLine($"Adding new set '{card.SetName}'...");
                setId = await _cardPrintService.InsertSetAsync(card.SetName);
            }
            else
            {
                Console.WriteLine($"Set '{card.SetName}' already exists!");

                setId = foundSet.Id;
            }

            // Insert card print if not found
            var foundCardPrint = await _cardPrintService.GetCardPrintDetailAsync(cardId, setId);
            if (foundCardPrint == null)
            {
                Console.WriteLine($"Adding new card print for '{card.Name}' in set '{card.SetName}'...");

                string pictureUrl = null;
                string flipPictureUrl = null;

                // Normal card picture
                if (card.ImageUri != null)
                {
                    pictureUrl = card.ImageUri.Normal;
                }
                // Flip card picture
                else if (card.ImageUri == null && card.CardFaces != null && card.CardFaces.Count == 2)
                {
                    pictureUrl = card.CardFaces[0].ImageUri?.Normal;
                    flipPictureUrl = card.CardFaces[1].ImageUri?.Normal;
                }

                await _cardPrintService.InsertCardPrintAsync(cardId, setId, pictureUrl, flipPictureUrl);
            }
        }
    }
}
