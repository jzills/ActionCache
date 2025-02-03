using ActionCache.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace ActionCache.AzureCosmos.Extensions;

/// <summary>
/// Extension methods for the <see cref="Container"/> class.
/// </summary>
internal static class ContainerExtensions
{
    /// <summary>
    /// Extension method for the <see cref="Container"/> class to retrieve a feed iterator
    /// for items with a specific partition key (Namespace).
    /// </summary>
    /// <param name="container">The <see cref="Container"/> to query in Cosmos DB.</param>
    /// <param name="namespace">The partition key (Namespace) to filter the query by.</param>
    /// <remarks>
    /// This method creates a LINQ query on the container's items, filters by the partition key
    /// (Namespace), and returns a feed iterator for the result set, which can be used to
    /// asynchronously retrieve the IDs of the matching items.
    /// </remarks>
    internal static FeedIterator<T> GetItemFeedIterator<T>(this Container container, Namespace @namespace) 
        where T : AzureCosmosEntry =>
            container.GetItemLinqQueryable<T>()
                .Where(item => item.Namespace == (string)@namespace)
                .ToFeedIterator();

    internal static async Task<IEnumerable<AzureCosmosEntry>> GetItemsAsync(this Container container, Namespace @namespace)
    {
        var itemIds = new List<AzureCosmosEntry>();
        var feedIterator = container.GetItemFeedIterator<AzureCosmosEntry>(@namespace);
        while (feedIterator.HasMoreResults)
        {
            var response = await feedIterator.ReadNextAsync();
            itemIds.AddRange(response.Resource);
        }

        return itemIds;
    }
}