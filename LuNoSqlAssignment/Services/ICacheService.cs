using LuNoSqlAssignment.Models;

namespace LuNoSqlAssignment.Services
{
    public interface ICacheService
    {       
        Task<bool> CreatePersonsCache(IList<Person> persons, RedisConnection? redisConnection);
        Task<bool> FlushAllPersonsFromCacheAsJsonValues(RedisConnection? redisConnection, int count = 0);
        Task<IList<Person>?> GetAllPersonsFromCacheAsJsonValues(RedisConnection? redisConnection, int count = 0);
        Task<bool> SetAllPersonsToCacheAsJsonValues(RedisConnection? redisConnection, IList<Person> persons);
        IList<Person> ChangeEntries(IList<Person> persons);
    }
}