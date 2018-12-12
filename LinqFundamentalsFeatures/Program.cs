using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqFundamentalsFeatures
{
	class Program
	{

		static void Main(string[] args)
		{
			FundamentalsPart1();
			QuerySyntax();
		}

		#region private

		private static void QuerySyntax()
		{
			string[] cities = {"Tacoma", "Seattle", "Portland", "Boise", "Salem", "Salom", "Eugene"};

			// query syntax ALWAYS starts with 'from' keyword
			IEnumerable<string> filteredCities =
				from city in cities
				where city.StartsWith("S") && city.Length < 7
				orderby city descending
				select city; // unlike SQL, the select clause comes at the end for Linq

			Action<string> write = x => Console.WriteLine(x);

			write($"Filtered cities: {string.Join(",", filteredCities.ToArray())}");
		}

		private static void FundamentalsPart1()
		{
			IEnumerable<Employee> developers = new Employee[]
			{
				new Employee {Id = 1, Name = "Scott"},
				new Employee {Id = 2, Name = "Chris"},
				new Employee {Id = 3, Name = "Maximillian"}
			};

			// using implicit typing
			var sales = new List<Employee>()
			{
				new Employee {Id = 4, Name = "Alex"},
				new Employee {Id = 5, Name = "Susanna"}
			};

			//foreach (var employee in developers.Where(NameStartsWithS))
			//{
			//	Console.WriteLine($"Name starts with 'S': {employee.Name}");
			//}

			foreach (var employee in developers.Where(e => e.Name.StartsWith("S"))) // returns t/f
			{
				Console.WriteLine($"Name starts with 'S': {employee.Name}");
			}

			// calling my extension methods
			Console.WriteLine($"Counts: {developers.Count()}");
			Console.WriteLine("Hello".LoremIpsum());

			// using enumerator to move thru the list
			IEnumerator<Employee> enumerator = sales.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Console.WriteLine(enumerator.Current.Name);
			}

			// basic Func<> syntax
			Func<int, int, int> f = Sum; // named method approach
			Func<int, int, int>
				add = (int x, int y) => x + y; // anon approach; type is optional but is a good idea for type checking
			Func<int, int, int, int> complex = (int x, int y, int z) =>
			{
				int temp = x * y;
				temp += z;
				// must have a return statement when using a method body i.e. 'braces'
				return temp;
			};

			// no return type on action, is void. Useful for 'doing' things that don't require return value
			Action<string> write = x => Console.WriteLine(x);

			int result = add(2, 3);
			write($"Add(2,3)... {result.ToString()}");
			write($"Complex(1,2,4): {complex(1, 2, 4).ToString()}");

			// method syntax
			var query = developers.Where(e => e.Name.Length == 5).OrderBy(e => e.Name).Select(e=>e); // the select is optional on the method syntax

			// query syntax (not all operations are available for query syntax so you would have to use method syntax)
			var query2 = from developer in developers 
				where developer.Name.Length == 5 
				orderby developer.Name
				select developer;

			foreach (var employee in query2)
			{
				write(employee.Name);
			}
		}

		private static int Sum(int x, int y)
		{
			return x + y;
		}

		// named method style
		private static bool NameStartsWithS(Employee employee)
		{
			return employee.Name.StartsWith("S");
		}

		#endregion
	}
}