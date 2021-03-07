using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookISBN.Models
{
	public class KullanıcıKitapModel
	{
		public string UserName { get; set; }
		public string BookName { get; set; }
		public string ISBN { get; set; }
		public DateTime Reserve { get; set; }
		public DateTime Deliver { get; set; }
	}
}