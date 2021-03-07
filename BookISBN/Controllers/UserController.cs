using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emgu.CV.OCR;
using BookISBN.Models;
using System.IO;
using System.Text.RegularExpressions;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace BookISBN.Controllers
{
	public class UserController : Controller
	{
		private dbContext context = new dbContext();

		// GET: User
		public ActionResult KitapArama()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Search(Image img)
		{
			if (img.BookName != null)
			{
				var book = context.Books.Where(i => i.Name == img.BookName).Select(i => new Image() { ISBN = i.ISBN }).FirstOrDefault();
				return View(book);
			}
			else if (img.File != null)
			{
				string fileName = Path.GetFileNameWithoutExtension(img.File.FileName);
				string extension = Path.GetExtension(img.File.FileName);
				fileName = img.BookName + extension;
				img.BookPath = Path.Combine(Server.MapPath("~/Images/"), fileName);
				img.File.SaveAs(img.BookPath);
				
				Mat m_originalImage = CvInvoke.Imread(img.BookPath, LoadImageType.Unchanged);
				Mat m_resultImage = new Mat();
				CvInvoke.CvtColor(m_originalImage, m_resultImage, ColorConversion.Bgr2Gray);
				CvInvoke.GaussianBlur(m_resultImage, m_resultImage, new System.Drawing.Size(5, 5), 0);
				CvInvoke.Threshold(m_resultImage, m_resultImage, 160, 255, ThresholdType.Binary);
				Tesseract tes = new Tesseract("C:\\tesseract-master\\", "tur", OcrEngineMode.TesseractOnly);
				tes.Recognize(m_resultImage);
				img.ISBN = tes.GetText();

				string[] words = Regex.Split(img.ISBN, "\r\n");
				foreach (string word in words)
				{
					if (word.Contains("ISBN") == true || word.Contains("İSBN") == true)
					{
						string[] splt = word.Split(' ');
						string rplc = splt[1].Replace('—', '-');
						img.ISBN = rplc;
					}
				}

				var book = context.Books.Where(i => i.ISBN == img.ISBN).Select(i => new Image() { BookName = i.Name }).FirstOrDefault();
				return View(book);
			}
			else
			{
				return View();
			}
		}

		public ActionResult KitapAlma()
		{
			var books = context.Books.ToList();
			return View(books);
		}

		public ActionResult Bought(int id)
		{
			int count = 0;
			DateTime date = DateTime.Now;
			int log = Convert.ToInt32(TempData["user"]);
			var supp = context.Supply.Select(i => i.BookId == id).FirstOrDefault();
			var user = context.Supply.Where(i => i.UserId == log).Count();
			var day = context.Supply.Where(i => i.UserId == log).Select(i => new { deliver = i.Deliver, reserve = i.Reserve });
			foreach (var item in day)
			{
				if (DateTime.Compare(date,item.deliver) > 0)
				{
					count++;
				}
			}
			
			if(supp == false && user < 3 && count == 0)
			{
				Supply supply = new Supply() { BookId = id, UserId = log, Reserve = date, Deliver = date.AddDays(7)};
				context.Books.Where(i => i.Id == id).FirstOrDefault().wasBought = true;
				context.Supply.Add(supply);
				context.SaveChanges();
				return RedirectToAction("KitapAlma","User");
			}
			else
			{
				return RedirectToAction("KitapAlma", "User");
			}
			
		}

		public ActionResult KitapVerme()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Return(Image img)
		{
			string fileName = Path.GetFileNameWithoutExtension(img.File.FileName);
			string extension = Path.GetExtension(img.File.FileName);
			fileName = img.BookName + extension;
			img.BookPath = Path.Combine(Server.MapPath("~/Images/"), fileName);
			img.File.SaveAs(img.BookPath);
			
			Mat m_originalImage = CvInvoke.Imread(img.BookPath, LoadImageType.Unchanged);
			Mat m_resultImage = new Mat();
			CvInvoke.CvtColor(m_originalImage, m_resultImage, ColorConversion.Bgr2Gray);
			CvInvoke.GaussianBlur(m_resultImage, m_resultImage, new System.Drawing.Size(5, 5), 0);
			CvInvoke.Threshold(m_resultImage, m_resultImage, 160, 255, ThresholdType.Binary);
			Tesseract tes = new Tesseract("C:\\tesseract-master\\", "tur", OcrEngineMode.TesseractOnly);
			tes.Recognize(m_resultImage);
			img.ISBN = tes.GetText();

			string[] words = Regex.Split(img.ISBN, "\r\n");
			foreach (string word in words)
			{
				if (word.Contains("ISBN") == true || word.Contains("İSBN") == true)
				{
					string[] splt = word.Split(' ');
					string rplc = splt[1].Replace('—', '-');
					img.ISBN = rplc;
				}
			}

			var book = context.Books.Where(i => i.ISBN == img.ISBN).Select(i => i.Id).FirstOrDefault();
			var supply = context.Supply.Where(i => i.BookId == book).FirstOrDefault();
			if(supply != null)
			{
				context.Books.Where(i => i.Id == book).FirstOrDefault().wasBought = false;
				context.Supply.Remove(supply);
				context.SaveChanges();
			}
			return RedirectToAction("KitapVerme", "User");
		}
	}
}