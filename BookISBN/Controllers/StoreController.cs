using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using BookISBN.Models;

namespace BookISBN.Controllers
{
	public class StoreController : Controller
	{
		private dbContext context = new dbContext();
		
		[HttpGet]
		public ActionResult Login()
		{
			//context.Database.Create();
			return View();
		}

		[HttpPost]
		public ActionResult Verify(User log)
		{
			var role = context.Users.Where(i => i.Name == log.Name && i.Password == log.Password).Select(i => new { Userid = i.Id, Role = i.Role }).FirstOrDefault();
			TempData["user"] = role.Userid;
			if (role.Role == "Admin")
			{
				return RedirectToAction("KitapEkle", "Admin");
			}
			else if(role.Role == "User")
			{
				return RedirectToAction("KitapArama", "User");
			}
			else
			{
				return RedirectToAction("Login","Store");
			}
		}
	}
}