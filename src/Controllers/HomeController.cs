using CacheManagerApi.CacheManager;
using Microsoft.AspNetCore.Mvc;

 
    [Route("home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICacheManager _cacheManager;


        public HomeController(ILogger<HomeController> logger,  ICacheManager cacheManager)
        {
            this._logger = logger;
            this._cacheManager = cacheManager;
        }

        /// <summary>
        /// CacheManager interface'i DI olarak consctructor ile inject edilir ve bu action'da Call edilir.
        /// </summary>
        /// <returns></returns>
        [Route("index"),HttpGet]
        public bool Index()
        {
            _cacheManager.Set<string>("test", "test",null, 1); 
            return true;
        }  

        /// <summary>
        /// CacheManager interface'i action seviyesinde inject edilir.
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <returns></returns>
        [Route("index2"),HttpGet]
        public bool Index2([FromServices]ICacheManager cacheManager)
        {
            cacheManager.Set<string>("test2", "test",null, 1); 
            return true;
        } 
    }
 