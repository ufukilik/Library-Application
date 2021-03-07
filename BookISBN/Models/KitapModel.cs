using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookISBN.Models
{
	public class KitapModel
	{
		public string BookName { get; set; }
		public string ISBN { get; set; }
		public DateTime Deliver { get; set; }
		public DateTime Reserve { get; set; }
	}
}