using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookISBN.Models
{
	public class Book
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string ISBN { get; set; }
		public bool wasBought { get; set; } // sonradan eklendi

		public ICollection<Supply> Supplies { get; set; }
	}
}