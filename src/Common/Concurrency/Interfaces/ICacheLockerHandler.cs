namespace ActionCache.Common.Concurrency;

public interface ICacheLockerHandler
{
    /// <summary>
    /// Asynchronously waits for a lock to be acquired, then executes an action if the lock is acquired.
    /// </summary>
    /// <param name="resource">The resource for which the lock is requested.</param>
    /// <param name="thenFunc">The action to be executed after the lock is acquired.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task WaitForLockThenAsync(string resource, Action thenFunc);

    /// <summary>
    /// Asynchronously waits for a lock to be acquired, then executes a function that returns a result if the lock is acquired.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the function.</typeparam>
    /// <param name="resource">The resource for which the lock is requested.</param>
    /// <param name="resultAccessor">The function that will return a result once the lock is acquired.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function or default value if lock is not acquired.</returns>
    Task<TResult?> WaitForLockThenAsync<TResult>(string resource, Func<TResult> resultAccessor);

    /// <summary>
    /// Waits for the lock to be acquired on the specified resource, then executes the provided action.
    /// </summary>
    /// <param name="resource">The resource to acquire the lock for.</param>
    /// <param name="thenFunc">The action to execute once the lock is acquired.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// The action will be executed after the lock is acquired and released once the action completes.
    /// If the lock acquisition fails, no action is executed.
    /// </remarks>
    Task WaitForLockThenAsync(string resource, Func<Task> thenFunc);

    /// <summary>
    /// Waits for the lock to be acquired on the specified resource, then executes the provided function and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the function.</typeparam>
    /// <param name="resource">The resource to acquire the lock for.</param>
    /// <param name="resultAccessor">The function that will be executed once the lock is acquired, which returns a result.</param>
    /// <returns>A task representing the asynchronous operation, with the result of the function.</returns>
    /// <remarks>
    /// The result will be returned after the lock is acquired and released once the function completes.
    /// If the lock acquisition fails, the result will be null.
    /// </remarks>
    Task<TResult?> WaitForLockThenAsync<TResult>(string resource, Func<Task<TResult>> resultAccessor);
}