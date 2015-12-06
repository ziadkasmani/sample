using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;

namespace HeyVoteSite.Controllers
{
    public class UploadController : System.Web.Http.ApiController
    {
        [System.Web.Http.HttpGet]
        public void xx()
        {
            string x = "ss";
        }

        [System.Web.Http.HttpPost]
        public string UploadProfile()
        {
            string result = "OK -->";
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var httpPostedFile = HttpContext.Current.Request.Files["file"];
                bool folderExists = Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadedFiles"));
                if (!folderExists)
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadedFiles"));
                var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), httpPostedFile.FileName);
                httpPostedFile.SaveAs(fileSavePath);

                if (File.Exists(fileSavePath) && System.IO.Path.GetExtension(fileSavePath) == ".mp4")
                {

                    string thumbpath, thumbname;
                    string thumbargs;
                    thumbpath = AppDomain.CurrentDomain.BaseDirectory + "UploadedFiles\\Thumb\\";
                    string guid = System.Guid.NewGuid().ToString();

                    thumbname = thumbpath + guid + ".jpg";
                    result = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/UploadedFiles/Thumb/" + guid + ".jpg";

                    string URL = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                    result = "{ \"thumb\" : \"" + URL + "/UploadedFiles/Thumb/" + guid + ".jpg" + "\" , \"VideoUrl\" : \"" + URL + "/UploadedFiles/" + httpPostedFile.FileName + "\"  }";

                    result = result.Replace("\\", "");


                    //result = thumbname;
                    thumbargs = "-i \"" + fileSavePath + "\" -vframes 1 -ss 00:00:02 -s 300x300 \"" + thumbname + "\"";
                    Process thumbproc = new Process();
                    thumbproc = new Process();
                    thumbproc.StartInfo.FileName = "C:\\tools\\ffmpeg.exe";
                    thumbproc.StartInfo.Arguments = thumbargs;
                    thumbproc.StartInfo.UseShellExecute = false;
                    thumbproc.StartInfo.CreateNoWindow = false;
                    thumbproc.StartInfo.RedirectStandardOutput = false;
                    try
                    {
                        thumbproc.Start();
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                    thumbproc.WaitForExit();
                    thumbproc.Close();
                }

            }
            return result;
        }

        [System.Web.Http.HttpPost]
        public string UploadFile()
        {
            string result = "OK -->";
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var httpPostedFile = HttpContext.Current.Request.Files["file"];
                bool folderExists = Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadedFiles"));
                if (!folderExists)
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadedFiles"));
                var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), httpPostedFile.FileName);
                httpPostedFile.SaveAs(fileSavePath);

                if (File.Exists(fileSavePath) && System.IO.Path.GetExtension(fileSavePath) == ".mp4")
                {

                    string thumbpath, thumbname;
                    string thumbargs;
                    thumbpath = AppDomain.CurrentDomain.BaseDirectory + "UploadedFiles\\Thumb\\";
                    string guid = System.Guid.NewGuid().ToString();

                    thumbname = thumbpath + guid + ".jpg";
                    result = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/UploadedFiles/Thumb/" + guid + ".jpg";

                    string URL = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                    result = "{ \"thumb\" : \"" + URL + "/UploadedFiles/Thumb/" + guid + ".jpg" + "\" , \"VideoUrl\" : \"" + URL + "/UploadedFiles/" + httpPostedFile.FileName + "\"  }";

                    result = result.Replace("\\", "");


                    //result = thumbname;
                    thumbargs = "-i \"" + fileSavePath + "\" -vframes 1 -ss 00:00:02 -s 300x300 \"" + thumbname + "\"";
                    Process thumbproc = new Process();
                    thumbproc = new Process();
                    thumbproc.StartInfo.FileName = "C:\\tools\\ffmpeg.exe";
                    thumbproc.StartInfo.Arguments = thumbargs;
                    thumbproc.StartInfo.UseShellExecute = false;
                    thumbproc.StartInfo.CreateNoWindow = false;
                    thumbproc.StartInfo.RedirectStandardOutput = false;
                    try
                    {
                        thumbproc.Start();
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                    thumbproc.WaitForExit();
                    thumbproc.Close();
                }

            }
            return result;
        }
    }
}