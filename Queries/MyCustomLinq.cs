using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queries
{
	public static class MyCustomLinq
	{
		// A TEMPLATE FOR HOW TO CREATE A LINQ EXTENSION METHOD
		// my custom filter, takes in a function and returns a bool
		public static IEnumerable<T> Filter<T>(this IEnumerable<T> source,
			Func<T, bool> predicate) // <= this is what is actually done.
		{
			foreach (var item in source)
			{
				if (predicate(item))
				{
					// DEFERRED EXECUTION (using yield return...)
					// to use 'yield', you must return an IEnumbrable
					// yield builds an IEnumberable data structure
					yield return item;
				}
			}
		}
	}
}
