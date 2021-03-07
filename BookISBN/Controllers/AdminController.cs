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
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace BookISBN.Controllers
{
    public class AdminController : Controller
    {
		private dbContext context = new dbContext();

		// GET: Admin
		public ActionResult KitapEkle()
        {
			return View();
		}

		[HttpPost]
		public ActionResult Add(Image img)
		{
			string fileName = Path.GetFileNameWithoutExtension(img.File.FileName);
			string extension = Path.GetExtension(img.File.FileName);
			fileName = img.BookName + extension;
			img.BookPath = Path.Combine(Server.MapPath("~/Images/"), fileName);
			img.File.SaveAs(img.BookPath);
			
			Mat m_originalImage = CvInvoke.Imread(img.BookPath, LoadImageType.Unchanged);
			Mat m_resultImage = new Mat();
			CvInvoke.CvtColor(m_originalImage, m_resultImage, ColorConversion.Bgr2Gray);
			m_resultImage.Save(Path.Combine(Server.MapPath("~/Images/"), "gray" + extension));
			CvInvoke.GaussianBlur(m_resultImage, m_resultImage, new System.Drawing.Size(5, 5), 0);
			CvInvoke.Threshold(m_resultImage, m_resultImage, 160, 255, ThresholdType.Binary);
			Tesseract tes = new Tesseract("C:\\tesseract-master\\", "tur", OcrEngineMode.TesseractOnly);
			tes.Recognize(m_resultImage);
			img.ISBN = tes.GetText();
			m_resultImage.Save(Path.Combine(Server.MapPath("~/Images/"), "threshold" + extension));
			
			string[] words = Regex.Split(img.ISBN,"\r\n");
			foreach (string word in words)
			{
				if(word.Contains("ISBN") == true || word.Contains("İSBN") == true)
				{
					string[] splt = word.Split(' ');
					string rplc = splt[1].Replace('—', '-');
					img.ISBN = rplc;
				}
			}

			Book book = new Book();
			book.Name = img.BookName;
			book.ISBN = img.ISBN;
			book.wasBought = false;
			
			context.Books.Add(book);
			context.SaveChanges();
			
			return View("KitapEkle");
		}

		public ActionResult ZamanAtlama()
		{
			return View();
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SYSTEMTIME
		{
			public short Year;
			public short Month;
			public short DayOfWeek;
			public short Day;
			public short Hour;
			public short Minute;
			public short Second;
			public short Milliseconds;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetSystemTime(ref SYSTEMTIME systime);

		[HttpPost]
		public ActionResult Time(Zaman daytime)
		{
			DateTime date = DateTime.Now;
			date = date.AddDays(daytime.Gun);
			SYSTEMTIME systime = new SYSTEMTIME();
			systime.Day = (short)date.Day;
			systime.DayOfWeek = (short)date.DayOfWeek;
			systime.Year = (short)date.Year;
			systime.Month = (short)date.Month;
			systime.Hour = (short)date.Hour;
			systime.Minute = (short)date.Minute;
			systime.Second = (short)date.Second;
			systime.Milliseconds = (short)date.Millisecond;
			SetSystemTime(ref systime);
			return RedirectToAction("ZamanAtlama","Admin"); 
		}
		
		public ActionResult KullaniciListeleme()
		{
			var user = context.Supply.Where(i => i.User.Role == "User").Select(i => new KullanıcıKitapModel()
			{
				UserName = i.User.Name,
				BookName = i.Book.Name,
				ISBN = i.Book.ISBN,
				Deliver = i.Deliver,
				Reserve = i.Reserve
			}).ToList();
			return View(user);
		}
	}
}