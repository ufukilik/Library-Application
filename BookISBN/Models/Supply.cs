using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookISBN.Models
{
	public class Supply
	{
		public int Id { get; set; }
		public DateTime Reserve { get; set; }
		public DateTime Deliver { get; set; }

		public int UserId { get; set; }
		public User User { get; set; }

		public int BookId { get; set; }
		public Book Book { get; set; }

	}
}