using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BookISBN.Models
{
	public class dbContext : DbContext
	{
		public dbContext() : base("dbConnection")
		{
			
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Book> Books { get; set; }
		public DbSet<Supply> Supply { get; set; }
	}
}