using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqFundamentalsFeatures
{
	// My custom extension methods but there could be name collision.
	public static class MyLinq
	{
		public static int Count<T>(this IEnumerable<T> sequence)
		{
			int count = 0;
			foreach (var item in sequence)
			{
				count += 1;
			}
			return count;
		}

		public static string LoremIpsum(this string value)
		{
			return $"{value} Lorem Ipsum valor...";
		}
	}
}
