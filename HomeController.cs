using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using HeyVoteClassLibrary.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace HeyVoteWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult MyPosts()
        {
            return View();
        }


        public ActionResult Territory()
        {
            return View();
        }


        public ActionResult MyVotes()
        {
            return View();
        }


        public ActionResult ViewPost()
        {
            return View();
        }

        public ActionResult ContactProfile()
        {
            return View();
        }

        public ActionResult Profile()
        {
            return View();
        }

        public ActionResult MyProfile()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        public ActionResult Following()
        {
            return View();
        }

        public ActionResult Contacts()
        {
            return View();
        }

        public ActionResult HeyVote()
        {
            return View();
        }
        public ActionResult HeyVoteIndex()
        {
            return View();
        }

        public ActionResult Logout()
        {
            RemoveCookie();
            return View("Index");
        }

        private void RemoveCookie()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "https://localhost");
            HttpContext.Response.AppendHeader("Access-Control-Allow-Credentials", "true");
            HttpContext.Response.SetCookie(new HttpCookie(AppConfigManager.CookieKey) { Path = "/HeyVoteWeb", Value = String.Empty, Secure = true, Expires = DateTime.Now.AddDays(-10) });
        }

        public ActionResult BarcodeImage(String barcodeText)
        {
            //Thread.Sleep(7000);
            // generating a barcode here.
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(barcodeText, out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(4, QuietZoneModules.Four), Brushes.Black, Brushes.White);

            Stream memoryStream = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);

            // very important to reset memory stream to a starting position, otherwise you would get 0 bytes returned
            memoryStream.Position = 0;

            var resultStream = new FileStreamResult(memoryStream, "image/png");
            resultStream.FileDownloadName = String.Format("{0}.png", barcodeText);

            return resultStream;
        }

        public FileResult GetImage(string id, string ff)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(String.Format(@"{0}/{1}/{2}.jpg", AppConfigManager.HeyFolderPath, ff, id));
                    string fileName = String.Format("{0}.jpg", id);
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = fileName,
                        Inline = true,
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                // changed here by yahya 
                //throw new Exception(CodeHelper.UnableToFindFile);
            }

            return null;
        }

        public ActionResult FS()
        {
            return View();
        }

    }
}