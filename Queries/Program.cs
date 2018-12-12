using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Queries
{
	class Program
	{
		private static List<Movie> _movies;
		
		static void Main(string[] args)
		{
			InitMovieData();
			Console.WriteLine("Showing by year >= 2000");
			ShowAfter2000();
			Console.WriteLine("Showing by Drama");
			ShowByCategory(CATEGORIES.Drama);
		}

		#region private

		private static void ShowAfter2000()
		{
			var query = _movies.Where(m => m.Year >= 2000).OrderBy(e => e.Year);
			foreach (var movie in query)
			{
				Console.WriteLine(ToJson(movie as object));
			}
		}

		private static void ShowByCategory(CATEGORIES category)
		{
			var query = _movies.Where(m => m.Category == category).OrderBy(m => m.Title);
			foreach (var movie in query)
			{
				Console.WriteLine(ToJson(movie as object));
			}
		}

		private static void InitMovieData()
		{
			_movies = new List<Movie>
			{
				new Movie { Title="The Dark Knight", Rating=8.9f, Year = 2008, Category=CATEGORIES.SciFi },
				new Movie { Title="The King's Speech", Rating=8.0f, Year=2010, Category=CATEGORIES.Drama },
				new Movie { Title="Casablanca", Rating=8.5f, Year=1942, Category=CATEGORIES.Drama },
				new Movie { Title="Star Wars V", Rating=8.7f, Year=1980, Category=CATEGORIES.SciFi },
				new Movie { Title="Blazing Saddles", Rating=7.5f, Year=1973, Category=CATEGORIES.Comedy }
			};
		}

		// expression body format (instead of block body)
		private static string ToJson(object o) => JsonConvert.SerializeObject(o);

		#endregion
	}
}
