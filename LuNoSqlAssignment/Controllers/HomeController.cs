using LuNoSqlAssignment.Models;
using LuNoSqlAssignment.Services;
using Microsoft.AspNetCore.Mvc;

using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;

namespace LuNoSqlAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration? _configuration;
        private readonly Task<RedisConnection> _redisConnectionFactory;
        private readonly IPersonsFileAccess _personsFileAccess;
        private readonly IWebHostEnvironment _env;
        private RedisConnection? _redisConnection;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, Task<RedisConnection> redisConnectionFactory, IPersonsFileAccess personsFileAccess, IWebHostEnvironment env)
        {
            _logger = logger;
            _configuration = configuration;
            _redisConnectionFactory = redisConnectionFactory;
            _personsFileAccess = personsFileAccess;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<ActionResult> RedisCache()
        {
            _redisConnection = await _redisConnectionFactory;
            ViewBag.Message = "A simple example with Azure Cache for Redis on ASP.NET Core.";

            // Perform cache operations using the cache object...

            // Simple PING command
            ViewBag.command1 = "PING";
            ViewBag.command1Result = (await _redisConnection.BasicRetryAsync(async (db) => await db.ExecuteAsync(ViewBag.command1))).ToString();

            // Simple get and put of integral data types into the cache
            string key = "Message";
            string value = "Hello! The cache is working from ASP.NET Core!";

            ViewBag.command2 = $"SET {key} \"{value}\"";
            ViewBag.command2Result = (await _redisConnection.BasicRetryAsync(async (db) => await db.StringSetAsync(key, value))).ToString();

            ViewBag.command3 = $"GET {key}";
            ViewBag.command3Result = (await _redisConnection.BasicRetryAsync(async (db) => await db.StringGetAsync(key))).ToString();

            key = "LastUpdateTime";
            value = DateTime.UtcNow.ToString();

            ViewBag.command4 = $"GET {key}";
            ViewBag.command4Result = (await _redisConnection.BasicRetryAsync(async (db) => await db.StringGetAsync(key))).ToString();

            ViewBag.command5 = $"SET {key}";
            ViewBag.command5Result = (await (Task<bool>)_redisConnection.BasicRetryAsync<bool>(async (db) => await (Task<bool>)db.StringSetAsync(key, value))).ToString();

            var mySetCommand1 = (await _redisConnection.BasicRetryAsync(async (db) => await db.StringSetAsync("name", "Vova"))).ToString();
            var myGetCommand1 = (await _redisConnection.BasicRetryAsync(async (db) => await db.StringGetAsync("name"))).ToString();
            //var myGetCommand1 = (await _redisConnection.BasicRetryAsync(async (db) => await db.inse

            //
            //var personsFromResources = PersonsFromResourcesFiles.Read();

            //_personsFileAccess.SavePersons(personsFromResources, "persons.txt", _env.WebRootPath);



            IList<Person> persons = _personsFileAccess.OpenPersons("persons.txt", _env.WebRootPath);

            var person = new Person();
            person.Name = "Vova";
            person.Surname = "Supervova";
            person.Address = new Address { HouseNumber = "3", Street = "mystreet", City = "myCity" };


            bool mySetCommandForPerson = await (Task<bool>)_redisConnection.BasicRetryAsync<bool>(async (db) => await (Task<bool>)db.HashSetAsync("persons:123", new HashEntry[] { new HashEntry("name", person.Name), new HashEntry("surname", person.Surname), new HashEntry("house", person.Address.HouseNumber), new HashEntry("street", person.Address.Street), new HashEntry("city", person.Address.City) }));

            var myGetCommandForPerson = await _redisConnection.BasicRetryAsync(async (db) => await db.HashGetAllAsync("persons:123"));

            var myGetCommandGetWithFilter = await _redisConnection.BasicRetryAsync(async (db) => await db.HashGetAsync("persons:123", "name"));

            var myGetCommandGetWithsearch = await _redisConnection.BasicRetryAsync(async (db) => await db.KeyDeleteAsync("persons:123"));

            var myGetCommandForPerson2 = await _redisConnection.BasicRetryAsync(async (db) => await db.HashGetAllAsync("persons:123"));

            var myDeleteDb = await _redisConnection.BasicRetryAsync((db) => Task.FromResult(db.CreateTransaction("dfdf")));

            //bool success1 = true;

            //for (int i = 0; i < persons.Count; i++)
            //{
            //    string currentPerson = $"persons:{i}";

            //    var getPerson = await _redisConnection.BasicRetryAsync(async (db) => await db.HashGetAllAsync(currentPerson));

            //    var delPerson = await _redisConnection.BasicRetryAsync(async (db) => await db.HashDeleteAsync(currentPerson, "name"));

            //    if (delPerson  == true)
            //    {
            //        success1 = false;
            //    }
            //}

            //var myGetPersonsAfterDelete = await _redisConnection.BasicRetryAsync(async (db) => await db.HashGetAllAsync());

            var jsper = JsonSerializer.Serialize(persons[5]);
            bool setPerson2 = await _redisConnection.BasicRetryAsync(async (db) => await db.SetAddAsync("ppl", JsonSerializer.Serialize(persons[5])));
            //var getPersonJsn = await _redisConnection.BasicRetryAsync(async (db) => await db.gets
            var getPerson2 = await _redisConnection.BasicRetryAsync(async (db) => await db.SetRemoveAsync("ppl", jsper));


            bool setPersonString = await _redisConnection.BasicRetryAsync(async (db) => await db.StringSetAsync("person:999", JsonSerializer.Serialize(persons[5])));

            var getPersonString = (await _redisConnection.BasicRetryAsync(async (db) => await db.StringGetAsync("person:999"))).ToString();

            Person? per = JsonSerializer.Deserialize<Person?>(getPersonString);

            var setJsonPersons = await setAllPersonsToCacheAsJsonValues(persons, _redisConnection);

            var getMultPersons = await getAllPersonsFromCacheAsJsonValues(_redisConnection, 3);

            var test = "";
            //var getPerson244 = (Task<bool>)_redisConnection.BasicRetryAsync<bool>(transaction);

            //var getPerson245 = (Task<bool>)_redisConnection.BasicRetryAsync<bool>(transaction);
            //var myGetPerson245 = await _redisConnection.BasicRetryAsync(async (db) => await db.HashGetAllAsync("personss:12"));
            //var myGetPerson246 = await _redisConnection.BasicRetryAsync(async (db) => await db.HashGetAllAsync("personss:13"));
            //var myGetPerson245 = await _redisConnection.BasicRetryAsync(async (db) => await db.HashGetAllAsync("personss:0"));
            // var myGetPerson246 = await _redisConnection.BasicRetryAsync(async (db) => await db.HashGetAllAsync("personss:1"));


            //    var getPerson2444 = _redisConnection.BasicRetryAsync(async (db) => await db.ScriptEvaluateAsync(@"if redis.call('hexists', KEYS[1], 'UniqueId') then return redis.call('hset', KEYS[1], 'UniqueId', ARGV[1]) else return 0 end", new RedisKey[] { "custKey" }, new RedisValue[] { "newId" }));


            //bool success2 = true;

            //for (int i = 0; i < persons.Count; i++)
            //{
            //    Person currentPerson = persons[i];

            //    bool setPerson = await (Task<bool>)_redisConnection.BasicRetryAsync<bool>(async (db) => await (Task<bool>)db.HashSetAsync($"persons:{i}", new HashEntry[] { new HashEntry("name", currentPerson.Name), new HashEntry("surname", currentPerson.Surname), new HashEntry("house", currentPerson.Address.HouseNumber), new HashEntry("street", currentPerson.Address.Street), new HashEntry("city", currentPerson.Address.City), new HashEntry("zip", currentPerson.Address.PostalCode) }));

            //    if (!setPerson)
            //    {
            //        success2 = false;
            //    }
            //}

            //persons[0].Name = "Kostja";

            bool created = await createPersonsCache(persons, _redisConnection);
            //var keys = new RedisKey[] { "persons:0", "persons:1" };

            //var myGetPerson = await _redisConnection.BasicRetryAsync(async (db) => await db.HashGetAllAsync(keys));

            var myGetPersonMatch = await _redisConnection.BasicRetryAsync((db) => Task.FromResult(db.HashScanAsync("persons", "na*")));



            return View();
        }


        private async Task<bool> createPersonsCache(IList<Person> persons, RedisConnection? redisConnection)
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
                var transactions = db.CreateTransaction();
                var tasks = new List<Task>();

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

                var committed = await transactions.ExecuteAsync();

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

        private async Task<bool> setAllPersonsToCacheAsJsonValues(IList<Person> persons, RedisConnection? redisConnection)
        {
            int count = 3;

            bool setPersons = await (Task<bool>)redisConnection.BasicRetryAsync<bool>(transaction);

            if (!setPersons)
            {
                return false;
            }

            async Task<bool> transaction(IDatabase db)
            {
                var transactions = db.CreateTransaction();
                var tasks = new List<Task>();

                for (int i = 0; i < count; i++)
                {
                    Person currentPerson = persons[i];

                    tasks.Add(transactions.StringSetAsync($"person:{i}", JsonSerializer.Serialize(persons[i])));
                }

                var committed = await transactions.ExecuteAsync();

                if (!committed)
                {
                    return false;
                }

                for (int i = 0; i < count; i++)
                {
                    await tasks[i];
                }

                return true;
            }

            return true;
        }

        private async Task<IList<Person>?> getAllPersonsFromCacheAsJsonValues(RedisConnection? redisConnection, int count)
        {            
            var getPersons = await (Task<IList<Person>>)redisConnection.BasicRetryAsync<IList<Person>>(transaction);

            async Task<IList<Person>> transaction(IDatabase db)
            {
                var transactions = db.CreateTransaction();
                var tasks = new List<Task<RedisValue>>();                

                for (int i = 0; i < count; i++)
                {
                    tasks.Add(transactions.StringGetAsync($"person:{i}"));
                }

                var committed = await transactions.ExecuteAsync();

                if (!committed)
                {
                    return null;
                }

                IList<Person?> persons = new List<Person>();

                for (int i = 0; i < count; i++)
                {
                    var person = (await tasks[i]);

                    var tt = person.ToString();

                    persons.Add(JsonSerializer.Deserialize<Person>(tt));
                }

                return persons;
            }

            return getPersons;
        }
    }
}