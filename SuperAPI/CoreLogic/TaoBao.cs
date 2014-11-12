﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Helper;
using LG.Utility;
using System.Web;
namespace CoreLogic {
    public class TaoBao : HostDo {
        public static List<string> Host{
            get { return new List<string> { ".taobao.com", ".tmall.com" }; }
        }
        /// <summary>
        /// 获取需要下载的图片集合
        /// </summary>
        /// <param name="appendAssist"></param>
        /// <returns></returns>
        public override List<string> GetImgs(string url,out string msg) {
            msg = string.Empty;
            var action = HttpContext.Current.Request.GetQ("action");
            if (action.IsNullOrWhiteSpace()) {
                msg = "action参数不能为空！";
                return null;
            }
            if (url.IsNullOrWhiteSpace()) {
                msg = "url参数不能为空！";
                return null;
            }
            switch (action) {
                case "detail": return DetailHander(url,out msg);//详情图片
                case "info": return InfoHander(url,out msg);//详情图片
            }
            msg = "What Are You Do？";
            return null;
        }
        /// <summary>
        /// 获取保存路径地址
        /// </summary>
        /// <param name="appendAssist"></param>
        /// <returns></returns>
        public override string GetSavePath(object appendAssist) {
            string id = string.Empty;
            if (appendAssist != null) id = (string)appendAssist;
            if (!DoCeck()) return string.Empty;
            string action=HttpContext.Current.Request.GetQ("action");
            string pos = HttpContext.Current.Request.GetQ("pos");
            pos = pos.IsNullOrWhiteSpace() ? "imgs" : pos;
            string type = "taobao";
            if(action.IsNullOrWhiteSpace())return string.Empty;
            var savePathStyle = HttpContext.Current.Server.MapPath("~/" + CommonConfig.SaveTaoBaoPathConfig.FormatStr(type, action, id, pos));
            return savePathStyle;
        }

        public override string GetUrl(object appendAssist) {
            string id = string.Empty;
            if (appendAssist != null) id = (string)appendAssist;
            if (!DoCeck()) return string.Empty;
            string action=HttpContext.Current.Request.GetQ("action");
            string pos = HttpContext.Current.Request.GetQ("pos");
            pos = pos.IsNullOrWhiteSpace() ? "imgs" : pos;
            string type = "taobao";
            if(action.IsNullOrWhiteSpace())return string.Empty;
            var savePathStyle = CommonConfig.TaoBaoUrlConfig.FormatStr(type, action, id, pos);
            return savePathStyle;
        }
        #region "获取详情"
        /// <summary>
        /// 获取详情图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<string> DetailHander(string url,out string msg) {
            //获取请求结果字符串
            msg = string.Empty;
            var resultStr = CommonHelper.DoGetRequest(url);
            if (resultStr.IsNullOrWhiteSpace()) {
                msg = "获取商品HTML失败！";
                return null;
            }
            //获取商品详细信息接口URL地址
            var descURL = GetDescDetailURL(resultStr);
            if (descURL.IsNullOrWhiteSpace()) {
                msg = "获取商品HTML为空！";
                return null;
            }
            //请求商品详细信息接口获取字符串
            var resultDescStr=CommonHelper.DoGetRequest(descURL);
            if (resultDescStr.IsNullOrWhiteSpace()) {
                msg = "获取商品详情数据接口数据字符串为空！";
                return null;
            }
            //根据详情数据字符串，获取图片集合
            var imgUrls=GetImgsByDetialStr(resultDescStr);
            return imgUrls;
        }
        /// <summary>
        /// 获取商品详情的数据URL
        /// </summary>
        /// <param name="taobaoItemHtml"></param>
        /// <returns></returns>
        private string GetDescDetailURL(string taobaoItemHtml) {
            if (taobaoItemHtml.IsNullOrWhiteSpace()) return string.Empty;
            string[] regexs = { @"""descUrl"":""([^""]*)"",",@"""apiItemDesc"":""([^""]*)""," };//天猫descUrl，淘宝apiItemDesc
            var detailDescUrl = string.Empty;
            foreach (var regItem in regexs) {
                var regex = new Regex(regItem, RegexOptions.Singleline);
                if (!regex.IsMatch(taobaoItemHtml)) continue;
                var match = regex.Match(taobaoItemHtml);
                if (match == null) continue;
                var groups = match.Groups;
                if (groups == null || groups.Count < 2) continue;
                detailDescUrl = groups[1].Value;
                break;
            }
            return detailDescUrl;
        }
        /// <summary>
        /// 获取图片地址集合
        /// </summary>
        /// <param name="detailRequestStr"></param>
        /// <returns></returns>
        private List<string> GetImgsByDetialStr(string detailRequestStr) {
            var datas = new List<string>();
            if (detailRequestStr.IsNullOrWhiteSpace()) return null;
            Regex regex = new Regex(@"<img[^>]*src=""([^""]*)""[^>]*>");
            var matchs = regex.Matches(detailRequestStr);
            if (matchs == null && matchs.Count<=0) return null;
            for (int i = 0; i < matchs.Count; i++) {
                var matchItem = matchs[i];
                if (matchItem == null) continue;
                var groups=matchItem.Groups;
                if (groups == null || groups.Count<2) continue;
                datas.Add(groups[1].Value);
            }
            return datas;
        }
        #endregion
        #region "商品主信息"
        public List<string> InfoHander(string url,out string msg) {
            msg = string.Empty;
            //<ul[\s]+id="J_UlThumb"[^>]*>.+?</ul>
            //<a href="#"><img data-src="([^"]*)"[^>]*>
            //获取请求结果字符串
            var resultStr = CommonHelper.DoGetRequest(url);
            if (resultStr.IsNullOrWhiteSpace()) {
                msg = "获取商品HTML为空！";
                return null;
            }
            var match=new Regex(@"<ul[^>]*id=""J_UlThumb""[^>]*>.+?</ul>",RegexOptions.Singleline).Match(resultStr);
            if (match == null || match.Length <= 0) {
                msg = @"解析商品HTML ul id=""J_UlThumb"" 标签字符串失败！";
                return null;
            }
            var imgsHTML = match.Groups[0].Value;
            string[] ImgRegexs = { @"<a[^>]*href=""#""[^>]*><img[^>]*src=""([^""]*)""[^>]*>", @"<a[^>]*href=""#""[^>]*><img[^>]*data-src=""([^""]*)""[^>]*>" };
            var datas = new List<string>();
            foreach (var item in ImgRegexs) {
                var imgMatchs = new Regex(item, RegexOptions.Singleline).Matches(imgsHTML);
                if (imgMatchs == null || imgMatchs.Count <= 0) {
                    msg = "解析img标签集合为空！";
                    return null;
                }
                for (int i = 0; i < imgMatchs.Count; i++) {
                    var matchItem = imgMatchs[i];
                    if (matchItem == null) continue;
                    var groups = matchItem.Groups;
                    if (groups == null || groups.Count < 2) continue;
                    var imgUrl = groups[1].Value.Replace("_60x60q90.jpg", "").Replace("_50x50.jpg", "");
                    datas.Add(imgUrl);
                }
                break;
            }
           return datas;
        }
        #endregion
        /// <summary>
        /// 校验是否合法参数
        /// </summary>
        /// <returns></returns>
        private bool DoCeck() {
            //合法action
            string[] legalAction = { "detail", "info" };
            //合法pos
            string[] legalPos = { "imgs", "pack" };
            string action = HttpContext.Current.Request.GetQ("action").ToLower();
            string pos = HttpContext.Current.Request.GetQ("pos").ToLower();
            if (legalAction.Contains(action) && (pos.IsNullOrWhiteSpace() || legalPos.Contains(pos))) return true;
            return false;
        }

      
    }
}
