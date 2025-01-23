using Microsoft.Azure.Cosmos;

namespace ActionCache.AzureCosmos.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an error occurs
/// fetching or creating an Azure Cosmos container. 
/// </summary>
public class AzureCosmosContainerNotFoundOrCreated : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCosmosContainerNotFoundOrCreated"/> class
    /// with the default error message.
    /// </summary>
    public AzureCosmosContainerNotFoundOrCreated(ContainerResponse response) 
        : base($"An error occurred fetching or creating Azure Cosmos container ({response.StatusCode}).")
    {
    }
}