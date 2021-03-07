﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookISBN.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }

		public ICollection<Supply> Supplies { get; set; }
	}
}