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
        public static string GetHttpContent(RequestType eType,string url,CookieCollection requestCookies,CookieCollection responseCookies,IDictionary<string, string> requestParames) {
            var rtnStr=string.Empty;
            switch(eType){
                case RequestType.GET: {
                    if (requestParames != null || requestParames.Count>0) {
                        StringBuilder paramesStr = new StringBuilder();
                        var i = 0;
                        var countNum = requestParames.Count;
                        foreach (var item in requestParames) {
                            var key=item.Key;
                            if(key.IsNullOrWhiteSpace())continue;
                            paramesStr.Append(key + "=" + item.Value);
                            if (i < (countNum - 1)) paramesStr.Append("&");
                            i++;
                        }
                    }
                    rtnStr= HttpAccessHelper.GetHttpGetResponseText(url,Encoding.UTF8,3000,requestCookies,out responseCookies);
                };break;
                case RequestType.POST: {
                    rtnStr = HttpAccessHelper.GetHttpPostResponseText(url, requestParames, null, null, false, Encoding.GetEncoding("utf-8"), 300000, requestCookies, out responseCookies);
                };break;
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
