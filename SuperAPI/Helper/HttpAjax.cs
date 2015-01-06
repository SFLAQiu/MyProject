using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using LG.Utility;
namespace Helper {
    public class HttpAjax {
        /// <summary>
        /// 获取请求内容
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="url"></param>
        /// <param name="requestCookies"></param>
        /// <param name="responseCookies"></param>
        /// <param name="requestParames"></param>
        /// <returns></returns>
        public static string GetHttpContent(RequestType eType, string url, CookieCollection requestCookies, CookieCollection responseCookies, IDictionary<string, string> requestParames, int timeoutMillisecond = 3000) {
            var rtnStr = string.Empty;
            switch (eType) {
                case RequestType.GET: {
                        if (requestParames != null && requestParames.Count > 0) {
                            StringBuilder paramesStr = new StringBuilder();
                            var i = 0;
                            var countNum = requestParames.Count;
                            foreach (var item in requestParames) {
                                var key = item.Key;
                                if (key.IsNullOrWhiteSpace()) continue;
                                paramesStr.Append(key + "=" + item.Value);
                                if (i < (countNum - 1)) paramesStr.Append("&");
                                i++;
                            }
                        }
                        try {
                            var postFn = new Func<string>(delegate() {
                                return  HttpAccessHelper.GetHttpGetResponseText(url, Encoding.UTF8, timeoutMillisecond, requestCookies, out responseCookies);
                            });
                            rtnStr =postFn.Invoke();
                            if (rtnStr.IsNullOrWhiteSpace()) rtnStr = postFn.Invoke();
                            if (rtnStr.IsNullOrWhiteSpace()) rtnStr = postFn.Invoke();
                        } catch { }
                    }; break;
                case RequestType.POST: {
                        try {
                            var postFn = new Func<string>(delegate() {
                                return HttpAccessHelper.GetHttpPostResponseText(url, requestParames, null, null, false, Encoding.GetEncoding("utf-8"), timeoutMillisecond, requestCookies, out responseCookies);
                            });
                            rtnStr = postFn();
                            if (rtnStr.IsNullOrWhiteSpace()) rtnStr = postFn();
                            if (rtnStr.IsNullOrWhiteSpace()) rtnStr = postFn();
                        } catch { }
                    }; break;
            }

            return rtnStr;
        }
    }
    /// <summary>
    /// 请求方式
    /// </summary>
    public enum RequestType { 
        GET=1,
        POST=2
    }
}
