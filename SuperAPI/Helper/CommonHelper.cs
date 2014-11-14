using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using LG.Utility;
using SwfDotNet.IO;
using SwfDotNet.IO.Tags;
using SwfDotNet.IO.Tags.Types;
namespace Helper {
    public class CommonHelper {
        /// <summary>
        /// 返回请求结果字符串
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string DoGetRequest(string url) {
            var resultStr = string.Empty;
            try {
                resultStr = HttpAccessHelper.GetHttpGetResponseText(url, Encoding.UTF8, 10000);
                if (resultStr.IsNullOrWhiteSpace()) HttpAccessHelper.GetHttpGetResponseText(url, Encoding.UTF8, 10000);
                if (resultStr.IsNullOrWhiteSpace()) HttpAccessHelper.GetHttpGetResponseText(url, Encoding.UTF8, 10000);
            } catch (Exception ex) {
                throw ex;
            }
            return resultStr;
        }
        
        /// <summary>
        /// 从图片地址下载图片到本地磁盘
        /// </summary>
        /// <param name="ToLocalPath">图片本地磁盘地址</param>
        /// <param name="url">图片网址</param>
        /// <returns></returns>
        public static bool SavePhotoFromUrl(string fileName, string url,out string errMsg) {
            errMsg = string.Empty;
            bool Value = false;
            WebResponse response = null;
            Stream stream = null;
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                response = request.GetResponse();
                stream = response.GetResponseStream();
                if (!response.ContentType.ToLower().StartsWith("text/")) Value = SaveBinaryFile(response, fileName);
            } catch (Exception err) {
                errMsg = err.ToString();
            }
            return Value;
        }
        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="response">The response used to save the file</param>
        // 将二进制文件保存到磁盘
        private static bool SaveBinaryFile(WebResponse response, string FileName) {
            bool Value = true;
            byte[] buffer = new byte[1024];
            try {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                Stream outStream = System.IO.File.Create(FileName);
                Stream inStream = response.GetResponseStream();
                int l;
                do {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);
                outStream.Close();
                inStream.Close();
            } catch {
                Value = false;
            }
            return Value;
        }
    }
}
