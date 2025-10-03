namespace Modern.Infrastructure.Entities
{
	public class Book
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public Book()
		{
			if (Id == null)
			{
				Id = Guid.NewGuid().ToString();
			}
	
		}
	}
}
