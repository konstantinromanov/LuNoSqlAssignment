using LuNoSqlAssignment.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace LuNoSqlAssignment.Services
{
    public class CacheService : ICacheService
    {
        private readonly IConfiguration? _configuration;

        public CacheService()
        {
        }

        public async Task<bool> SetAllPersonsToCacheAsJsonValues(RedisConnection? redisConnection, IList<Person> persons)
        {
            bool setPersons = await (Task<bool>)redisConnection.BasicRetryAsync<bool>(transaction);

            if (!setPersons)
            {
                return false;
            }

            async Task<bool> transaction(IDatabase db)
            {
                ITransaction? transactions = db.CreateTransaction();
                List<Task>? tasks = new List<Task>();

                for (int i = 0; i < persons.Count; i++)
                {
                    persons[i].Id = i;
                    tasks.Add(transactions.StringSetAsync($"person:{i}", JsonSerializer.Serialize(persons[i])));
                }

                bool committed = await transactions.ExecuteAsync();

                if (!committed)
                {
                    return false;
                }

                for (int i = 0; i < persons.Count; i++)
                {
                    await tasks[i];
                }

                bool counter = (await redisConnection.BasicRetryAsync(async (db) => await db.StringSetAsync("personsNumber", persons.Count.ToString())));
                
                return true;
            }

            return true;
        }

        public async Task<IList<Person>?> GetAllPersonsFromCacheAsJsonValues(RedisConnection? redisConnection, int count = 0)
        {
            int numberOfPersons = Int32.Parse((await redisConnection.BasicRetryAsync((db) => db.StringGetAsync("personsNumber"))).ToString());
            count = count == 0 ? numberOfPersons : count;            
            IList<Person>? getPersons = await (Task<IList<Person>>)redisConnection.BasicRetryAsync<IList<Person>>(transaction);

            async Task<IList<Person>> transaction(IDatabase db)
            {
                ITransaction? transactions = db.CreateTransaction();
                List<Task<RedisValue>>? tasks = new List<Task<RedisValue>>();

                for (int i = 0; i < count; i++)
                {
                    tasks.Add(transactions.StringGetAsync($"person:{i}"));
                }

                bool committed = await transactions.ExecuteAsync();

                if (!committed)
                {
                    return null;
                }

                IList<Person> persons = new List<Person>();

                for (int i = 0; i < count; i++)
                {
                    persons.Add(JsonSerializer.Deserialize<Person>((await tasks[i]).ToString()));
                }

                return persons;
            }

            return getPersons;
        }

        public async Task<bool> FlushAllPersonsFromCacheAsJsonValues(RedisConnection? redisConnection, int count = 0)
        {
            int numberInCache = Int32.Parse((await redisConnection.BasicRetryAsync(async (db) => await db.StringGetAsync("personsNumber"))).ToString());

            int numberToPop = count == 0 ? numberInCache : count;
            int startToFlushFrom = count == 0 ? 0 : numberInCache - count;
            bool personsFlushed = await (Task<bool>)redisConnection.BasicRetryAsync<bool>(transaction);            

            async Task<bool> transaction(IDatabase db)
            {
                ITransaction? transactions = db.CreateTransaction();
                List<Task<bool>>? tasks = new List<Task<bool>>();

                for (int i = 0; i < numberToPop; i++)
                {
                    int indexToDelete = startToFlushFrom + i;
                    tasks.Add(transactions.KeyDeleteAsync($"person:{indexToDelete}"));
                }

                bool committed = await transactions.ExecuteAsync();

                if (!committed)
                {
                    return false;
                }

                for (int i = 0; i < numberToPop; i++)
                {
                    await tasks[i];
                }

                bool counter = (await redisConnection.BasicRetryAsync(async (db) => await db.StringSetAsync("personsNumber", (numberInCache - numberToPop).ToString())));
                
                return true;
            }

            return true;
        }

        public IList<Person> ChangeEntries(IList<Person> persons)
        {
            persons[0].Address.City = "Riga";
            persons[1].Address.City = "Liepaja";
            persons[2].Address.City = "Ventspils";
            persons[3].Address.City = "Jelgava";
            persons[4].Address.City = "Aluksne";
            persons[5].Address.City = "Rezekne";
            persons[6].Address.City = "Valmiera";
            persons[7].Address.City = "Grobina";
            persons[8].Address.City = "Bauska";
            persons[9].Address.City = "Skrunda";
            persons[10].Address.City = "Kuldiga";

            persons[0].Address.PostalCode = "LV-1013";
            persons[1].Address.PostalCode = "LV-3405";
            persons[2].Address.PostalCode = "LV-3602";
            persons[3].Address.PostalCode = "LV-3001";
            persons[4].Address.PostalCode = "LV-4301";
            persons[5].Address.PostalCode = "LV-4604";
            persons[6].Address.PostalCode = "LV-4204";
            persons[7].Address.PostalCode = "LV-3430";
            persons[8].Address.PostalCode = "LV-3901";
            persons[9].Address.PostalCode = "LV-3326";
            persons[10].Address.PostalCode = "LV-3301";

            for (int i = 0; i < 10; i++)
            {
                persons[i].Address.Street = "Brivibas";
            }

            return persons;
        }

        // This method is not being used, it was created to show another approach to store object as hash (commented out code is for storing, left one is for deletion).
        public async Task<bool> CreatePersonsCache(IList<Person> persons, RedisConnection? redisConnection)
        {
            if (redisConnection == null)
            {
                return false;
            }

            bool setPersons = await (Task<bool>)redisConnection.BasicRetryAsync<bool>(transaction);

            if (!setPersons)
            {
                return false;
            }

            async Task<bool> transaction(IDatabase db)
            {
                ITransaction? transactions = db.CreateTransaction();
                List<Task>? tasks = new List<Task>();

                for (int i = 0; i < persons.Count; i++)
                {
                    Person currentPerson = persons[i];

                    //tasks.Add((Task)transactions.HashSetAsync($"persons:{i}", new HashEntry[] { new HashEntry("name", currentPerson.Name), new HashEntry("surname", currentPerson.Surname), new HashEntry("house", currentPerson.Address.HouseNumber), new HashEntry("street", currentPerson.Address.Street), new HashEntry("city", currentPerson.Address.City), new HashEntry("zip", currentPerson.Address.PostalCode) }));

                    tasks.Add((Task)transactions.HashDeleteAsync($"persons:{i}", "name"));
                    tasks.Add((Task)transactions.HashDeleteAsync($"persons:{i}", "surname"));
                    tasks.Add((Task)transactions.HashDeleteAsync($"persons:{i}", "house"));
                    tasks.Add((Task)transactions.HashDeleteAsync($"persons:{i}", "street"));
                    tasks.Add((Task)transactions.HashDeleteAsync($"persons:{i}", "city"));
                    tasks.Add((Task)transactions.HashDeleteAsync($"persons:{i}", "zip"));

                    tasks.Add((Task)transactions.KeyDeleteAsync($"persons:{i}"));
                }

                bool committed = await transactions.ExecuteAsync();

                if (!committed)
                {
                    return false;
                }

                for (int i = 0; i < persons.Count; i++)
                {
                    await tasks[i];
                }

                return true;
            }

            return true;
        }
    }
}
