namespace Queries
{
	class Movie
	{
		public string Title { get; set; }
		public float Rating { get; set; }
		public int Year { get; set; }
		public CATEGORIES Category { get; set; }
	}

	public enum CATEGORIES
	{
		None,
		Comedy,
		SciFi,
		Fantasy,
		Horror,
		Drama
	}
}
