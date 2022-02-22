using DataAccess.Models;

namespace DataAccess.Services
{
    public interface ICollectionService
    {
        /// <summary>
        /// Adds a new collection.
        /// </summary>
        /// <returns></returns>
        Task<int> AddCollectionAsync(AddCollectionRequest request);

        /// <summary>
        /// Adds a sideboard to a mainboard deck.
        /// </summary>
        /// <param name="mainboardId"></param>
        /// <returns></returns>
        Task<int> AddDeckSideboardAsync(int mainboardId);

        /// <summary>
        /// Adds an owned card.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<int> AddOwnedCardAsync(OwnedCardRequest request);

        /// <summary>
        /// Deletes the given number of matching card prints in the given set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="numberToDelete"></param>
        /// <returns></returns>
        Task DeleteOwnedCardsAsync(OwnedCardRequest request, int numberToDelete);


        /// <summary>
        /// Gets a collection by the Id.
        /// </summary>
        /// <param name="collectionId"></param>
        /// <returns>Returns the found collection. If no collection is found, will return a null value.</returns>
        Task<CardCollection> GetCollectionAsync(int collectionId);

        /// <summary>
        /// Gets all collections.
        /// </summary>
        /// <returns>Returns all collections. If no collections are found, will return an empty collection.</returns>
        Task<IEnumerable<CardCollection>> GetCollectionsAsync();

        /// <summary>
        /// Gets the aggregate owned details by card Id.
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns>Returns a collection. If no entries are found, will return an empty collection.</returns>
        Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsyncByCardId(int cardId);

        /// <summary>
        /// Gets the aggregate owned card details by collection Id.
        /// </summary>
        /// <returns>Returns a collection. If no entries are found, will return an empty collection.</returns>
        Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsyncByCollectionId(int collectionId);
    }
}
