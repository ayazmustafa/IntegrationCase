using System.Collections.Concurrent;
using Integration.Common;
using Integration.Backend;
using StackExchange.Redis;

namespace Integration.Service;

public sealed class ItemIntegrationService
{
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();
    private readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect("localhost");
    private readonly IDatabase _database;

    public ItemIntegrationService()
    {
        _database = _redis.GetDatabase();
    }

    public Result SaveItem(string itemContent)
    {
        // Try to acquire a distributed lock
        var acquiredLock = _database.LockTake(itemContent, itemContent, TimeSpan.FromSeconds(10));

        if (!acquiredLock)
        {
            throw new RedisServerException($"Could not acquire lock for key => '{itemContent}'");
        }

        try
        {
            if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
            {
                return new Result(false, $"Duplicate item received with content {itemContent}.");
            }

            var item = ItemIntegrationBackend.SaveItem(itemContent);

            return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
        }
        finally
        {
            _database.LockRelease(itemContent, itemContent);
        }
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
}