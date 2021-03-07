using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookISBN.Models
{
	public class Image
	{
		public string BookName { get; set; }
		public string BookPath { get; set; }
		public string ISBN { get; set; }
		public HttpPostedFileBase File { get; set; }
	}
}