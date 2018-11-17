using System;
using System.IO;
using System.Net;
using System.Web;

namespace CL
{
    public class FileHelper
    {
        private static string _basePath = "";

        /// <summary>
        /// Get absolutePath by relativePath
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentNullException("the relativePath couldn't be null");
            }
            relativePath = relativePath.Replace("/", "\\");
            if (relativePath[0] == '\\')
            {
                relativePath = relativePath.Remove(0, 1);
            }
            if (HttpContext.Current != null)
            {
                return Path.Combine(HttpRuntime.AppDomainAppPath, relativePath);
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }
        }




        /// <summary>
        /// download file
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path">if null then set to the [current domain's base directory / downloads / yyyyMMdd]</param>
        /// <param name="fileName">if null then set to the [Guid.NewGuid().ToString().Replace("-", "")]</param>
        /// <returns>the whole saved path with fileName</returns>
        public static string DownLoadFile(string url, string path = null, string fileName = null)
        {
            try
            {
                using (Stream stream = WebRequest.Create(url).GetResponse().GetResponseStream())
                {
                    if (string.IsNullOrEmpty(path))
                        path = _basePath;
                    if (string.IsNullOrEmpty(fileName))
                        fileName = Guid.NewGuid().ToString().Replace("-", "");

                    string extension = Path.GetExtension(url);
                    extension = extension.Split('?')[0];
                    if (string.IsNullOrEmpty(extension))
                        extension = GetImageExtensionFromStream(stream);

                    CheckDirectoryExist(path);

                    string savePath = string.Format("{0}\\{1}{2}", path, fileName, extension);

                    using (FileStream fs = new FileStream(savePath, FileMode.Create))
                    {
                        int bufferSize = 2048;
                        byte[] bytes = new byte[bufferSize];
                        int length = stream.Read(bytes, 0, bufferSize);

                        while (length > 0)
                        {
                            fs.Write(bytes, 0, length);
                            length = stream.Read(bytes, 0, bufferSize);
                        }
                    }

                    return savePath;
                }
            }
            catch (Exception ex)
            {
                //Logger.Log4Net.Error("save file from url error: " + ex.Message);
                throw ex;
            }
        }


        public static string DownloadUrlContent(string url)
        {
            byte[] content = null;

            content = new WebClient().DownloadData(url);
            return System.Text.Encoding.UTF8.GetString(content);
        }


        /// <summary>
        /// check the directory exist or not
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isCreate">true: if not exist, creat then return true</param>
        /// <returns></returns>
        public static bool CheckDirectoryExist(string path, bool isCreate = true)
        {
            if (Directory.Exists(path))
            {
                return true;
            }

            if (isCreate)
            {
                Directory.CreateDirectory(path);
                return true;
            }

            return false;
        }


        private static string GetImageExtensionFromStream(Stream stream)
        {
            string strImgFormat = "";
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(stream))
            {
                if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                    strImgFormat = ".jpeg";
                else if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                    strImgFormat = ".gif";
                else if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                    strImgFormat = ".bmp";
                else if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                    strImgFormat = ".png";
                else if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                    strImgFormat = ".tiff";
                else if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                    strImgFormat = ".icon";
                else if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Wmf))
                    strImgFormat = ".wmf";
                else
                    strImgFormat = string.Empty;
                //{
                //    Logger.Log4Net.Error("Unknow Image Format");
                //    throw new FormatException();
                //}

                return strImgFormat;
            }
        }


        #region downLoad
        //public static void DownloadByWebClient(string url)
        //{
        //    string fileName = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMddHHmmss") + "webClient";
        //    WebClient hc = new WebClient();
        //    hc.DownloadFile(url, fileName);
        //}

        //public bool DownloadByHttpRequest(string path)
        //{
        //    long speed = 1024000;
        //    HttpRequest request = HttpContext.Current.Request;
        //    HttpResponse response = HttpContext.Current.Response;
        //    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(path);

        //    FileStream myFile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //    BinaryReader br = new BinaryReader(myFile);
        //    try
        //    {
        //        response.AddHeader("Accept-Ranges", "bytes");
        //        response.Buffer = false;
        //        long fileLength = myFile.Length;
        //        long startBytes = 0;

        //        double pack = 10240; //10K bytes
        //        //int sleep = 200;   //每秒5次   即5*10K bytes每秒
        //        int sleep = (int)Math.Floor(1000 * pack / speed) + 1;
        //        if (request.Headers["Range"] != null)
        //        {
        //            response.StatusCode = 206;
        //            string[] range = request.Headers["Range"].Split(new char[] { '=', '-' });
        //            startBytes = Convert.ToInt64(range[1]);
        //        }
        //        response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
        //        response.AddHeader("Connection", "Keep-Alive");
        //        response.ContentType = "application/octet-stream";
        //        response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));

        //        br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
        //        int maxCount = (int)Math.Floor((fileLength - startBytes) / pack) + 1;

        //        for (int i = 0; i < maxCount; i++)
        //        {
        //            if (response.IsClientConnected)
        //            {
        //                response.BinaryWrite(br.ReadBytes(int.Parse(pack.ToString())));
        //                System.Threading.Thread.Sleep(sleep);
        //            }
        //            else
        //            {
        //                i = maxCount;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        br.Close();
        //        myFile.Close();
        //    }
        //    return true;
        //}
        #endregion
    }
}
