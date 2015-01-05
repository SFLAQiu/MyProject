﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LG.Utility;
using CoreLogic;
using Helper;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Net;
namespace Web.Controllers {
    public partial class AjaxController : Base {
        #region "查询商家接口"
        /// <summary>
        /// 关键词排名查询
        /// </summary>
        /// <returns></returns>
        public ActionResult MallKeywordRanking(){
            var responseContent = HttpAjax.GetHttpContent(RequestType.POST, "http://tool.58pic.com/search/index.php?a=grabInfo", GetLoginCookie(), null,
            new Dictionary<string, string> { 
                {"account","%E4%BA%94%E5%8D%83%E9%94%A6%E7%BB%A3%E6%97%97%E8%88%B0%E5%BA%97"},//淘宝账号
                {"keyword","%E4%BA%94%E5%8D%83%E9%94%A6%E7%BB%A3%E6%97%97%E8%88%B0%E5%BA%97"},//宝贝关键词
                {"sort","default"},//排序方式
                {"maxpage","1"},//排名
                {"curpage","1"}//排名
            });
            return Content(responseContent, "text/html",Encoding.UTF8);
        }



        /// <summary>
        /// 动态获取商家信息（http://tool.58pic.com/dynamic/）
        /// </summary>
        /// <returns></returns>
        public ActionResult DynamicPingJia() {
            string a = Request.GetQ("a");
            string m = Request.GetQ("m");
            string requestUrl = @"http://tool.58pic.com/dynamic/?m={0}&a={1}".FormatStr(a, m);
            var responseContent = string.Empty;
            switch (a) {
                case "search": responseContent = GetRequestPingJiaContent(requestUrl); break;
                case "jis": responseContent = GetRequestJSContent(requestUrl); break;
            }
            return Content(responseContent, "text/html", Encoding.UTF8);
        }
        #endregion
        #region "千图网辅助"
        /// <summary>
        /// 获取评价数据
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        private string GetRequestPingJiaContent(string requestUrl) {
#if DEBUG
            string searchname = Request.GetQ("searchname");//掌柜名
#else 
            string searchname = Request.GetF("searchname");
#endif
            return HttpAjax.GetHttpContent(
                RequestType.POST, 
                requestUrl, 
                GetLoginCookie(), 
                null,
                new Dictionary<string, string> { 
                    {"searchname",searchname}
                }
            );
        }

        /// <summary>
        /// 获取评价计算数据
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        private string GetRequestJSContent(string requestUrl) {
#if DEBUG
            string url = Request.GetQ("url");
            string num = Request.GetQ("num");
#else 
            string url = Request.GetF("utl");
            string num = Request.GetF("num");
#endif
            return  HttpAjax.GetHttpContent(
                RequestType.POST, 
                requestUrl, 
                GetLoginCookie(), 
                null,
                new Dictionary<string, string> { 
                    {"url",url},
                    {"num",num}
                }
            );
        }
        /// <summary>
        /// 千图网登陆凭据
        /// </summary>
        /// <returns></returns>
        public CookieCollection GetLoginCookie() {
            CookieCollection cookies = new CookieCollection();
            cookies.Add(new Cookie {
                Domain = ".58pic.com",
                Expires = DateTime.Now.AddYears(10),
                Path = "/",
                Name = "auth_id",
                Value = "%224343094%7CSFLYQ%7C1421214756%7C95f12d2411ee3a2725af9b5e996d934e%22"
            });
            cookies.Add(new Cookie {
                Domain = ".58pic.com",
                Expires = DateTime.Now.AddYears(10),
                Path = "/",
                Name = "sns",
                Value = "%7B%22token%22%3A%7B%22access_token%22%3A%2288FE11E0C05541B0A795D1103BCB7BC8%22%2C%22expires_in%22%3A%227776000%22%2C%22refresh_token%22%3A%22A32A69454B9422B366339C25CFDCCCAB%22%2C%22openid%22%3A%22917951441771A2AF489D5B1B35C13A15%22%7D%2C%22type%22%3A%22qq%22%7D"
            });
            cookies.Add(new Cookie {
                Domain = ".58pic.com",
                Expires = DateTime.Now.AddYears(10),
                Path = "/",
                Name = "ssid",
                Value = "%2254a8d524657ba5.18363909%22"
            });
            cookies.Add(new Cookie {
                Domain = ".58pic.com",
                Expires = DateTime.Now.AddYears(10),
                Path = "/",
                Name = "tbt_sign",
                Value = "%229%7C1420359108%22"
            });
            return cookies;
        }
        #endregion
    }
}
