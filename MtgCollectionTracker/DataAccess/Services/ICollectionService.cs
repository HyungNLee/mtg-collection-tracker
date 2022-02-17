using DataAccess.Models;

namespace DataAccess.Services
{
    public interface ICollectionService
    {
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
