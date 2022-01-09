using Caching.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caching.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IDistributedCache distributedCache;
        private readonly IMemoryCache memoryCache;
        private readonly bool isMemoryCache = true;

        public MovieController(IDistributedCache _distributedCache,
                                IMemoryCache _memoryCache)
        {
            this.distributedCache = _distributedCache;
            this.memoryCache = _memoryCache;
        }

        [HttpGet]
        public async Task<List<string>> Get()
        {
            return isMemoryCache == true ? await GetMovieList_Memory() : await GetMovieList_Distributed();
        }
        private async Task<List<string>> GetMovieList_Distributed()
        {
            var cacheKey = "popular";

            List<string> moviesList;
            string serializedMovies;

            var encodedMovies = await distributedCache.GetAsync(cacheKey);

            if (encodedMovies != null)
            {
                serializedMovies = Encoding.UTF8.GetString(encodedMovies);
                moviesList = JsonConvert.DeserializeObject<List<string>>(serializedMovies);
            }
            else
            {
                moviesList = await MovieAPICall.GetMovieList();
                serializedMovies = JsonConvert.SerializeObject(moviesList);
                encodedMovies = Encoding.UTF8.GetBytes(serializedMovies);
                var options = new DistributedCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                                .SetAbsoluteExpiration(DateTime.Now.AddHours(6));
                await distributedCache.SetAsync(cacheKey, encodedMovies, options);
            }
            return moviesList;
        }

        private async Task<List<string>> GetMovieList_Memory()
        {
            var cacheKey = "popular";
            if (!memoryCache.TryGetValue(cacheKey, out List<string> movieList))
            {
                movieList = await MovieAPICall.GetMovieList();
                var cacheExpirationOptions =
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddHours(6),
                        Priority = CacheItemPriority.Normal,
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    };
                memoryCache.Set(cacheKey, movieList, cacheExpirationOptions);
            }
            return movieList;
        }

        [HttpGet("[action]")]
        public async Task<List<string>> Test()
        {
            string key = "popular";
            memoryCache.Remove(key);

            int count = 100;
            var list = new List<string>();
            
            Stopwatch watch = new Stopwatch();
            #region In Memory Cache
            watch.Start();
            for (int i = 0; i < count; i++)
            {
                await GetMovieList_Memory();
            }
            watch.Stop();
            double time = watch.Elapsed.TotalSeconds;
            list.Add($"In-memory caching: {time.ToString()} seconds");
            #endregion

            #region No Cache
            watch.Restart();
            for (int i = 0; i < count; i++)
            {
                await MovieAPICall.GetMovieList();
            }
            watch.Stop();
            time = watch.Elapsed.TotalSeconds;
            list.Add($"No cache: {time.ToString()} seconds"); 
            #endregion
            return list;
        }

    }
}
