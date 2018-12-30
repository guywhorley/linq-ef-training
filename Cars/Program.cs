using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Cars
{
	class Program
	{
		static List<Car> cars;
		static List<Manufacturer> manufacturers;

		static void Main(string[] args)
		{
			Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
			InsertData();
			QueryData();

			#region older chapters

			//CreateFuelXml();
			//QueryXMl();

			//InitTestData(); // transform csv into car objects
			//CarsLinq(cars);
			//CarJoinCode(cars, manufacturers);
			//CarGrouping(cars, manufacturers);
			//CarGroupJoin(cars, manufacturers);
			//AggregateData(cars, manufacturers);

			#endregion

			Console.WriteLine("Done with test run. Press enter key...");
			Console.ReadLine();
		}

		private static void QueryData()
		{
			// get an instance of CarDb
			var db = new CarDb();
			// TODO: Example of using Database.Log to writeout db operations
			db.Database.Log = Console.WriteLine;

			// define the query with query syntax
			//var query =
			//	from car in db.Cars
			//	orderby car.Combined descending, car.Name ascending
			//	select car;

			// Extension Method Syntax
			var query2 =
				db.Cars.OrderByDescending(c => c.Combined).ThenBy(c => c.Name).Take(10);

			foreach (var car in query2) //.Take(10))
			{
				Console.WriteLine($"{car.Name} : {car.Combined}");
			}
		}

		private static void InsertData()
		{
			//var cars = ProcessCars("fuel.csv");
			var db = new CarDb();
			db.Database.Log = Console.WriteLine;
			if (!db.Cars.Any())
			{
				var cars = ProcessCars("fuel.csv");
				foreach (var car in cars)
				{
					db.Cars.Add(car);
				}
				// insert the cars into the table.
				db.SaveChanges();
			}
		}

		// TODO: Query XML Document via LINQ
		// TODO: Using namespaces in query
		private static void QueryXMl()
		{
			var ns = (XNamespace)"http://whogu01.com/cars/2018";
			var ex = (XNamespace) "http://whogu01.com/cars/2018/ex";

			var document = XDocument.Load("fuel.xml"); // get xml file into memory
			var query =
				//from element in document.Descendants("Car") // all cars in document
				// following is example of using top level element and navigating explicitly.
				// LINQ stitches together the namespace and knows how to get at the right elements
				// TODO: Example of null coelescing and returning and empty sequence.
				// TODO: Example of RETURN AND EMPTY LIST OF IENUMERABLE<T>
				from element in document.Element(ns + "Cars")?.Elements(ex + "Car") ?? Enumerable.Empty<XElement>()
				// .Elements("Car") returns an IEnumberable, thus we can use LINQ From to iteate over the collection.
				where element.Attribute("Manufacturer")?.Value == "BMW"
				select element.Attribute("Name")?.Value;
			foreach (var name in query)
			{
				Console.WriteLine($"Cars made by BMW: {name}");
			}
		}

		// TODO: Example of XML API => CREATING XML DOCUMENT
		// Using XmlDocument Api approach (No LINQ)
		private static void CreateFuelXml()
		{ 
			// TODO: Example of using a namespace 
			// convert to XNamespace class
			var ns = (XNamespace)"http://whogu01.com/cars/2018";
			var ex = (XNamespace) "http://whogu01.com/cars/2018/ex";

			var records = ProcessCars("fuel.csv");
			var document = new XDocument();
			// TODO: using LINQ to create compact code for xml doc
			// TODO: Replacing foreach with a LINQ select
			var cars = new XElement(ns + "Cars", // this syntax MUST be used for namespace (no interpoation)
							from record in records
							// using projection for FUNCTIONAL CONSTRUCTION
							select new XElement(ex + "Car", // must include namespace
									new XAttribute("Name", record.Name),
									new XAttribute("Highway", record.Highway),
									new XAttribute("Combined", record.Combined),
									new XAttribute("Manufacturer", record.Manufacturer)));
			// add namespace prefix
			cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));
			
			// create the document
			document.Add(cars);
			document.Save("fuel.xml");
		}

		/// <summary>
		/// Sum, count, average, min, max
		/// </summary>
		/// <param name="cars"></param>
		/// <param name="manufacturers"></param>
		private static void AggregateData(List<Car> cars, List<Manufacturer> manufacturers)
		{
			//TODO: LINQ AGGREGATION: MIN,MAX,AVG,COUNT
			// QUERY SYNTAX
			// calculate min, max, avg
			var query =
				from car in cars
				group car by car.Manufacturer
				into carGroup
				select new
				{
					// Looping 3x
					Count = carGroup.Count(),
					Name = carGroup.Key,
					Max = carGroup.Max(c => c.Combined),
					Min = carGroup.Min(c => c.Combined),
					Avg = carGroup.Average(c => c.Combined)
				} into result
				orderby result.Max descending
				select result;

			var query2 =
				cars.GroupBy(c => c.Manufacturer)
					.Select(g =>
					{
						// only looping 1x due to aggregator
						var results = g.Aggregate(new CarStatistics(),
							(acc, c) => acc.Accumulate(c),
							acc => acc.Compute());
						return new
						{
							//
							Name = g.Key,
							Avg = results.Average,
							Max = results.Max,
							Min = results.Min,
							Count = results.Count
						};
					})
					.OrderByDescending(r=>r.Max);


			foreach (var result in query2)
			{
				Console.WriteLine($"{result.Name}");
				Console.WriteLine($"\t Max: {result.Max}");
				Console.WriteLine($"\t Min: {result.Min}");
				Console.WriteLine($"\t Avg: {result.Avg}");
				Console.WriteLine($"\t Cnt: {result.Count}");
			}
		}

		private static void CarGroupJoin(List<Car> cars, List<Manufacturer> manufacturers)
		{
			// TODO: LINQ GROUP-JOIN
			// Query-Method Syntax for group join
			var query =
				from manufacturer in manufacturers
				join car in cars on manufacturer.Name equals car.Manufacturer
					into carGroup
				orderby manufacturer.Name
				select new // Project the data
				{
					Manufacturer = manufacturer,
					Cars = carGroup
				}
				into result
				group result by result.Manufacturer.Headquarters;

			foreach (var group in query)
			{
				Console.WriteLine($"{group.Key}");
				foreach (var car in group.SelectMany(g=>g.Cars) // flatten
										 .OrderByDescending(c=>c.Combined)
										 .Take(3))
				{
					Console.WriteLine($"\t{car.Name} : {car.Combined}");
				}
			}
			
			// extension method syntax for group join
			var query2 =
				manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer, (m, g) =>
						new
						{
							Manufacturer = m,
							Cars = g
						})
					.OrderBy(m => m.Manufacturer.Name);

			foreach (var group in query2)
			{
				Console.WriteLine($"{group.Manufacturer.Name}:{group.Manufacturer.Headquarters}");
				foreach (var car in group.Cars.OrderByDescending(c=>c.Combined).Take(2))
				{
					Console.WriteLine($"\t{car.Name} : {car.Combined}");
				}
			}
		}

		private static void CarGrouping(List<Car> cars, object munufacturers)
		{
			//TODO: LINQ GROUPING, ORDERBY
			// Query-Method Syntax
			var query =
				from car in cars
				group car by car.Manufacturer.ToUpper()
				into manufacturer // placed into new dto so you can orderBy
				orderby manufacturer.Key
				select manufacturer;

			// Extension-Method Syntax
			var query2 =
				cars.GroupBy(c => c.Manufacturer.ToUpper())
					.OrderBy(g => g.Key);

			foreach (var group in query2)
			{
				Console.WriteLine(group.Key);
				foreach (var car in group.OrderByDescending(c=>c.Combined).Take(2))
				{
					Console.WriteLine($"\t{car.Name} : {car.Combined}");
				}
			}
		}

		private static void CarJoinCode(List<Car> cars, List<Manufacturer> manufacturers)
		{
			// TODO: LINQ JOIN
			// QUERY-SYNTAX - THIS IS THE PATTERN TO USE SINCE IT IS EASIER TO WRITE
			// THAN THE EXTENSION-METHOD APPROACH
			// INNER JOIN - *** IF right side is missing *** (i.e. no match on m.Name), it does not make it to the final results;
			var car_man =
				from car in cars
				join m in manufacturers
					//on car.Manufacturer equals m.Name // NOTE: must use 'equals' keyword on the join 'on'
					on new { car.Manufacturer, car.Year }
					equals
					new { Manufacturer = m.Name, m.Year } // prop names must match, using two join props
				orderby car.Combined descending, car.Name ascending
				select new //transform into a projection which now has Headquarters
				{
					m.Headquarters,
					car.Name,
					car.Combined
				};

			// JOIN EXTENSION-METHOD SYNTAX - A slightly more complex join pattern
			var car_man_alt =
				cars.Join(manufacturers, // 1. Join cars to manufacturers
										 //c => c.Manufacturer, // 2. user these two props to link the tables
										 //m => m.Name, (c, m) => new // 3. create a new third object to contain the following
							c => new { c.Manufacturer, c.Year },
							m => new { Manufacturer = m.Name, m.Year }, // JOIN ON TWO PROPS
							(c, m) => new
							{
								m.Headquarters,
								c.Name,
								c.Combined
							}) // now, you can contine but you must use the new third object
					.OrderByDescending(c => c.Combined)
					.ThenBy(c => c.Name);

			foreach (var car in car_man_alt.Take(10)) { Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}"); }
		}

		private static void InitTestData()
		{
			cars = ProcessCars("fuel.csv");
			manufacturers = ProcessManufacturers("manufacturers.csv");
		}

		private static void CarsLinq(List<Car> cars)
		{
			// query syntax
			var query1 =
				from car in cars
				where car.Manufacturer == "BMW" && car.Year == 2016
				orderby car.Combined descending, car.Name
				//select car; // project
				select new // project the full car into a leaner car
				{
					car.Manufacturer,
					car.Name,
					car.Combined
				};

			// Benchmark the query
			Stopwatch sw = new Stopwatch();
			sw.Start();

			// FirstOrDefault() ... LastOrDefault() ... First()... Last()...
			var top =
				cars
					.OrderByDescending(c => c.Combined)
					.ThenBy(c => c.Name)
					.Select(c => c)
					.FirstOrDefault(c => c.Manufacturer == "BMW" && c.Year == 2016); // immediate execution
			sw.Stop();
			Console.WriteLine($"Elapsed time for query = {sw.ElapsedTicks.ToString()}");
			Console.WriteLine($"top: name={top?.Name} : Combined={top?.Combined} mpg");

			// Any
			var result = cars.Any(c => c.Manufacturer == "Ford");
			Console.WriteLine($"Any cars are Ford: {result}");

			// All
			result = cars.All(c => c.Manufacturer == "Ford");
			Console.WriteLine($"All cars are Ford: {result}");

			// TRANSFORM INLINE ANONYMOUS OBJECT (a custom DTO)
			// Transform cars from one structure into another
			//var transformed = cars.Select(c => new {c.Name, c.Manufacturer, c.Combined});
			//foreach (var car in transformed.Take(10)) // the transformed object is now known as car.
			//{
			//	Console.WriteLine($"car: [Name:{car.Name}] [Maker:{car.Manufacturer}] [Combined:{car.Combined} mpg]");
			//}
			
			// extension-method syntax
			//var query1 = cars.OrderByDescending(c => c.Combined)
			// .ThenBy(c => c.Name); // secondary sort

			//foreach (var car in query1.Take(10))
			//{
			//	Console.WriteLine($"{car.Name} : {car.Combined}");
			//}
		}

		private static List<Car> ProcessCars(string path)
		{
			// query syntax
			var query =
				File.ReadAllLines(path)
					.Skip(1)
					.Where(l => l.Length > 1)
					.ToCar();//.Select(l => Car.ParseFromCsv(l));

				//from line in File.ReadAllLines(path).Skip(1)
				//where line.Length > 1
				//select Car.ParseFromCsv(line); // projection
			return query.ToList();

			// extension-method syntax
			//return File.ReadAllLines(path)
			// .Skip(1)
			// .Where(line => line.Length > 1)
			// .Select(Car.ParseFromCsv)
			// .ToList();
		}

		private static List<Manufacturer> ProcessManufacturers(string path)
		{
			var query =
				File.ReadAllLines(path)
					.Where(l => l.Length > 1)
					.Select(l =>
					{
						var columns = l.Split(',');
						return new Manufacturer
						{
							Name = columns[0],
							Headquarters = columns[1],
							Year = int.Parse(columns[2])
						};
					});
			return query.ToList();
		}
	}
	
	public static class CarExtensions
    {       
		// Transform i.e. project a line from car csv into a new car
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }

	// For car aggregation
	public class CarStatistics
	{
		public CarStatistics()
		{
			Max = int.MinValue;
			Min = int.MaxValue;
		}

		public int Total { get; set; }
		public int Max { get; set; }
		public int Min { get; set; }
		public double Average { get; set; }
		public int Count { get; set; }

		public CarStatistics Accumulate(Car car)
		{
			Total += car.Combined;
			Count += 1;
			Max = Math.Max(Max, car.Combined);
			Min = Math.Min(Min, car.Combined);
			return this;
		}

		public CarStatistics Compute()
		{
			Average = Total / Count;
			return this;
		}

	}

}