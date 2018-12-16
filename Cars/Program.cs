using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
	class Program
	{
		static void Main(string[] args)
		{
		
			// transform csv into car objects
			var cars = ProcessCars("fuel.csv");

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

			// Transform cars from one structure into another
			var transformed = cars.Select(c => new {c.Name, c.Manufacturer, c.Combined});
			foreach (var car in transformed.Take(10))
			{
				Console.WriteLine($"car: [Name:{car.Name}] [Maker:{car.Manufacturer}] [Combined:{car.Combined} mpg]");
			}
			
			return;
			
			// extension-method syntax
			//var query1 = cars.OrderByDescending(c => c.Combined)
			// .ThenBy(c => c.Name); // secondary sort

			foreach (var car in query1.Take(10))
			{
				Console.WriteLine($"{car.Name} : {car.Combined}");
			}

			//return;


			var manufacturers = ProcessManufacturers("manufacturers.csv");
			var query =
				from car in cars
				where car.Manufacturer == "BMW" && car.Year == 2016
				orderby car.Combined descending, car.Name ascending
				select new //transform
				{
					car.Manufacturer,
					car.Name,
					car.Combined
				};

			foreach (var car in query.Take(10))
			{
				Console.WriteLine($"{car.Manufacturer} {car.Name} : {car.Combined}");
			}
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
}