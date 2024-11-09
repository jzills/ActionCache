<link rel="stylesheet" href="README.css">

<p class="logo-p">
  <img src="../resources/Icon.jpg" alt="Zills">
  <span>ActionCache</span>
</p>

[![NuGet Version](https://img.shields.io/nuget/v/ActionCache.svg)](https://www.nuget.org/packages/ActionCache/) [![NuGet Downloads](https://img.shields.io/nuget/dt/ActionCache.svg)](https://www.nuget.org/packages/ActionCache/)

- [Quickstart](#quickstart)
    * [Register with IMemoryCache](#register-with-imemorycache)
    * [Register with Redis](#register-with-redis)
    * [Register both IMemoryCache and Redis](#register-both-imemorycache-and-redis)
    * [Basic Usage](#basic-usage)
    * [Cache Key Creation](#cache-key-creation)
    * [Cache Eviction](#cache-eviction)

## Register with IMemoryCache

Use the `AddActionCacheMemory` extension method to register `IMemoryCache` as the cache store. The configuration for `MemoryCacheOptions` is exposed as an optional parameter.

    builder.Services.AddActionCacheMemory();

## Register with Redis

Use the `AddActionCacheRedis` extension method to register Redis as the cache store. The configuration for `RedisCacheOptions` is exposed as an optional parameter.

    builder.Services.AddActionCacheRedis(options => 
        options.Configuration = ...);

## Register both IMemoryCache and Redis

The two caches can be combined. The `IMemoryCache` instance will be checked for a cache hit before Redis.

    builder.Services.AddActionCacheMemory();
    builder.Services.AddActionCacheRedis(options => 
        options.Configuration = ...);

## Basic Usage

Add an `ActionCacheAttribute` to any controller actions that should be cached. There is a mandatory parameter for the cache namespace which will prefix all entries with whatever is specified.

    [HttpPost]
    [Route("/")]
    [ActionCache(Namespace = "MyNamespace")]
    public IActionResult Post() 
    {
    }

> [!IMPORTANT]
> The current implementation only supports action return types of `IActionResult`. Specifically, the action filter that populates the cache looks for an `OkObjectResult` returned from the controller action.

## Cache Key Creation

Both the route values and the action arguments are serialized then encoded to generate the cache key suffix. This suffix is appended to the string "ActionCache:{Namespace}".

> [!NOTE]
> Any route data from the request, i.e. the area, controller and action names are also added to the key. This is helpful for the case of automatic cache rehydration which will be part of a future release.

## Cache Eviction

An `ActionCacheEvictionAttribute` can be applied to a controller action. Cache eviction occurs at the namespace level. One or more namespaces can be used separated by a comma. In the example below, both *MyNamespace* and *MyOtherNamespace* would have their entries evicted on a successful execution of the action.

    [HttpDelete]
    [Route("/")]
    [ActionCacheEviction(Namespace = "MyNamespace, MyOtherNamespace")]
    public IActionResult Delete()
    {
    }