using Caching.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Caching.Helpers
{
    public static class MovieAPICall
    {
        const string url = "https://api.themoviedb.org/3/";
        const string apiKey = "your-apikey";
        const string language = "en-US";

        public async static Task<List<string>> GetMovieList()
        {
            var result = new List<string>();

            string urlParameters = $"{url}movie/popular?api_key={apiKey}&language={language}";
            var movieList = await GetAsync<Movies>(url, urlParameters);
            if (movieList != null)
            {
                result = movieList.results.Select(r => r.Title).ToList();
            }
            return result;
        }
        private static HttpClient GetHttpClient(string url)
        {
            var client = new HttpClient { BaseAddress = new Uri(url) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private static async Task<T> GetAsync<T>(string url, string urlParameters)
        {
            try
            {
                using (var client = GetHttpClient(url))
                {
                    HttpResponseMessage response = await client.GetAsync(urlParameters);

                    #region  StatusCode is NOT OK
                    if (!response.StatusCode.Equals(HttpStatusCode.OK))
                        return default;

                    #endregion
                    #region StatusCode is OK
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<T>(json);
                    return result; 
                    #endregion
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
