using LuNoSqlAssignment.Models;
using LuNoSqlAssignment.Properties;
using LuNoSqlAssignment.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Resources;

namespace LuNoSqlAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration? _configuration;
        private readonly Task<RedisConnection> _redisConnectionFactory;
        private readonly IPersonsFileAccess _personsFileAccess;
        private RedisConnection? _redisConnection;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, Task<RedisConnection> redisConnectionFactory, IPersonsFileAccess personsFileAccess)
        {
            _logger = logger;
            _configuration = configuration;
            _redisConnectionFactory = redisConnectionFactory;
            _personsFileAccess = personsFileAccess;
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
            ViewBag.command5Result = (await _redisConnection.BasicRetryAsync(async (db) => await db.StringSetAsync(key, value))).ToString();

            var mySetCommand1 = (await _redisConnection.BasicRetryAsync(async (db) => await db.StringSetAsync("name", "Vova"))).ToString();
            var myGetCommand1 = (await _redisConnection.BasicRetryAsync(async (db) => await db.StringGetAsync("name"))).ToString();
            //var myGetCommand1 = (await _redisConnection.BasicRetryAsync(async (db) => await db.inse

            var myNames = ResourcesNames.Jonathon;





            List<Person> persons = new List<Person>();


            ResourceSet? resourceSet = ResourcesNames.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            if (resourceSet != null)
            {
                foreach (DictionaryEntry item in resourceSet)
                {
                    persons.Add(new Person { Name = (string?)item.Key, Surname = (string?)item.Value });
                }
            }

            ResourceSet? resourceSetAddress = ResourcesAddresses.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            if (resourceSetAddress != null)
            {
                int i = 0;

                foreach (DictionaryEntry item in resourceSetAddress)
                {
                    persons[i++].Address = new Address { HouseNumber = (string?)item.Key, Street = (string?)item.Value };
                }
            }

            ResourceSet? resourceSetAddress2 = ResourcesAddresses2.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            if (resourceSetAddress2 != null)
            {
                int i = 0;

                foreach (DictionaryEntry item in resourceSetAddress2)
                {
                    if (persons[i].Address != null)
                    {
                        persons[i].Address.City = (string?)item.Key;
                        persons[i++].Address.PostalCode = (string?)item.Value;
                    }

                }
            }

            _personsFileAccess.SavePersons(persons, "persons.txt");


            return View();
        }
    }
}