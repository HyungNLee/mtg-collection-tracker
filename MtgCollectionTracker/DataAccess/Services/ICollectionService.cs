using DataAccess.Models;

namespace DataAccess.Services
{
    public interface ICollectionService
    {
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
        /// Gets all collections.
        /// </summary>
        /// <returns>Returns all collections. If no collections are found, will return an empty collection.</returns>
        Task<IEnumerable<CardCollection>> GetCollectionsAsync();

        /// <summary>
        /// Gets the aggregate owned card details by collection Id.
        /// </summary>
        /// <returns>Returns a collection. If no entries are found, will return an empty collection.</returns>
        Task<IEnumerable<OwnedCardPrintAggregate>> GetOwnedCardsAggregatesAsync(int collectionId);
    }
}
