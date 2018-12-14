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
			Infinite();

			//Console.WriteLine("Showing by Drama");
			//ShowByCategory(CATEGORIES.Drama);
		}

		#region private

		private static void Infinite()
		{
			var numbers = MyCustomLinq.Random().Where(n => n > 0.5).Take(10);
			foreach (var number in numbers)
			{
				Console.WriteLine(number);
			}

		}

		

		private static void ShowAfter2000()
		{	
			// extension methods - define the query
			var query = _movies
				.Where(m => m.Year >= 2000)
				.OrderByDescending(m => m.Rating);

			

			//.ToList(); // <== This 'turns off' deferred execution.

			// any .To<Something> will immediately run the query and place the results into the list.
			// No more query wil be run at this point whenever referencing query in later code execute the query.

			//foreach (var movie in query) { Console.WriteLine(ToJson(movie as object)); }

			// Count is not a deferred operation!!!
			//Console.WriteLine($"Query Count: {query.Count()}");

			try
			{
				var enumerator = query.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Console.WriteLine(enumerator.Current.Title);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}

		private static void ShowByCategory(CATEGORIES category)
		{
			// extension method
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
