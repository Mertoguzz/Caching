using Newtonsoft.Json;
using System.Collections.Generic;

namespace Caching.Models
{
    public class Movie
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        //public string Year { get; set; }
        //public string Rated { get; set; }
        //public string Released { get; set; }
        //public string Runtime { get; set; }
        //public string Genre { get; set; }
        //public string Director { get; set; }
        //public string Writer { get; set; }
        //public string Actors { get; set; }
        //public string Plot { get; set; }
        //public string Language { get; set; }
        //public string Country { get; set; }
        //public string Awards { get; set; }
        //public string Poster { get; set; }
        ////public List<object> Ratings { get; set; }
        //public string Metascore { get; set; }
        //public string imdbRating { get; set; }
        //public string imdbVotes { get; set; }
        //public string imdbID { get; set; }
        //public string Type { get; set; }
        //public string totalSeasons { get; set; }
       // public string Response { get; set; }
    }

    public class Movies
    {
        public int page { get; set; }
        public List<Movie> results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}