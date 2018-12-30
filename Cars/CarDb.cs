using System.Data.Entity;

namespace Cars
{
	// TODO: Entity Framework Conventions
	// EF will use the ns and db class name as follows (unless there is explicit commands)
	// create a database called 'CarDb' in a namespace 'Cars' for a db of 'Cars.CarDb' and use 
	// the Car.Id as the primary key (identity column)

	// Database 'CarDb'
	class CarDb : DbContext
	{
		// Table in DB called 'Cars'
		public DbSet<Car> Cars { get; set; }



	}
}
