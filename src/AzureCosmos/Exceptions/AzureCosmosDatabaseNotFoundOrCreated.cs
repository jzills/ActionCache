using Microsoft.Azure.Cosmos;

namespace ActionCache.AzureCosmos.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an error occurs
/// fetching or creating an Azure Cosmos database. 
/// </summary>
public class AzureCosmosDatabaseNotFoundOrCreated : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCosmosDatabaseNotFoundOrCreated"/> class
    /// with the default error message.
    /// </summary>
    public AzureCosmosDatabaseNotFoundOrCreated(DatabaseResponse response) 
        : base($"An error occurred fetching or creating Azure Cosmos database ({response.StatusCode}).")
    {
    }
}