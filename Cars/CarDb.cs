using System.Data.Entity;

namespace Cars
{
	// Database 'CarDb'
	class CarDb : DbContext
	{
		// Table in DB called 'Cars'
		public DbSet<Car> Cars { get; set; }



	}
}
