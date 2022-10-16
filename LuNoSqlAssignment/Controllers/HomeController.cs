using LuNoSqlAssignment.Models;
using LuNoSqlAssignment.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LuNoSqlAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration? _configuration;
        private readonly Task<RedisConnection> _redisConnectionFactory;
        private readonly IPersonsFileAccess _personsFileAccess;
        private readonly IWebHostEnvironment _env;
        private RedisConnection? _redisConnection;
        private const int _totalNumberOfEntries = 500;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment env, Task<RedisConnection> redisConnectionFactory, IPersonsFileAccess personsFileAccess, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _configuration = configuration;
            _redisConnectionFactory = redisConnectionFactory;
            _personsFileAccess = personsFileAccess;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> DisplayPersonsEntries()
        {
            _redisConnection = await _redisConnectionFactory;

            try
            {
                IList<Person>? persons = await _cacheService.GetAllPersonsFromCacheAsJsonValues(_redisConnection);

                return View(persons);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> SetFifeHudredToCache()
        {
            _redisConnection = await _redisConnectionFactory;

            // This code is for creating .txt file in Json format from the resources files with persons data.
            //var personsFromResources = PersonsFromResourcesFiles.Read();
            //_personsFileAccess.SavePersons(personsFromResources, "persons.txt", _env.WebRootPath);

            IList<Person> persons = _personsFileAccess.OpenPersons("persons.txt", _env.WebRootPath);

            bool setCompleted = await _cacheService.SetAllPersonsToCacheAsJsonValues(_redisConnection, persons);

            if (setCompleted)
            {
                return RedirectToAction("DisplayPersonsEntries");
            }

            return View();
        }

        public async Task<IActionResult> FlushFifeHudredFromCache()
        {
            _redisConnection = await _redisConnectionFactory;

            bool flushedSuccessfully = await _cacheService.FlushAllPersonsFromCacheAsJsonValues(_redisConnection);

            if (flushedSuccessfully)
            {
                return RedirectToAction("DisplayPersonsEntries");
            }

            return RedirectToAction("DisplayPersonsEntries");
        }

        public async Task<IActionResult> FlushOneHudredFromCache()
        {
            _redisConnection = await _redisConnectionFactory;

            bool flushedSuccessfully = await _cacheService.FlushAllPersonsFromCacheAsJsonValues(_redisConnection, 150);

            if (flushedSuccessfully)
            {
                return RedirectToAction("DisplayPersonsEntries");
            }

            return RedirectToAction("DisplayPersonsEntries");
        }

        public async Task<IActionResult> FetchFifteenFromCache()
        {
            _redisConnection = await _redisConnectionFactory;

            try
            {
                IList<Person>? persons = await _cacheService.GetAllPersonsFromCacheAsJsonValues(_redisConnection, 15);

                return View("DisplayPersonsEntries", persons);
            }
            catch (Exception)
            {
                return RedirectToAction("DisplayPersonsEntries");
            }
        }

        public async Task<IActionResult> ChangeEntriesInCache()
        {
            _redisConnection = await _redisConnectionFactory;

            try
            {
                IList<Person>? persons = await _cacheService.GetAllPersonsFromCacheAsJsonValues(_redisConnection);
                persons = _cacheService.ChangeEntries(persons);
                bool setCompleted = await _cacheService.SetAllPersonsToCacheAsJsonValues(_redisConnection, persons);

                return View("DisplayPersonsEntries", persons);
            }
            catch (Exception)
            {
                return RedirectToAction("DisplayPersonsEntries");
            }
        }
    }
}