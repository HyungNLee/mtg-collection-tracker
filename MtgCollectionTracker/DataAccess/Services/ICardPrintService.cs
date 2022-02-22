using DataAccess.Models;

namespace DataAccess.Services
{
    public interface ICardPrintService
    {
        /// <summary>
        /// Gets a card by the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns the found card. Will return null if no card is found.</returns>
        Task<Card> GetCardAsync(string name);

        /// <summary>
        /// Gets a card print by card Id and set Id.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="setId"></param>
        /// <returns>Returns the found card print. Will return null if no card is found.</returns>
        Task<CardPrintDetail> GetCardPrintDetailAsync(int cardId, int setId);

        /// <summary>
        /// Gets a card print by card name and set name.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="setId"></param>
        /// <returns>Returns the found card print. Will return null if no card is found.</returns>
        Task<CardPrintDetail> GetCardPrintDetailAsync(string cardName, string setName);

        /// <summary>
        /// Gets all card print details.
        /// </summary>
        /// <returns>A collection of all card print details.</returns>
        Task<IEnumerable<CardPrintDetail>> GetCardPrintDetailsAsync();

        /// <summary>
        /// Gets a set by the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns the found set. Will return null if no set is found.</returns>
        Task<Set> GetSetAsync(string name);

        /// <summary>
        /// Inserts a new card.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<int> InsertCardAsync(string name);

        /// <summary>
        /// Inserts a new card print.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="setId"></param>
        /// <param name="pictureUrl"></param>
        /// <returns></returns>
        Task<int> InsertCardPrintAsync(int cardId, int setId, string pictureUrl, string flipPictureUrl);

        /// <summary>
        /// Inserts a new set.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<int> InsertSetAsync(string name);
    }
}
