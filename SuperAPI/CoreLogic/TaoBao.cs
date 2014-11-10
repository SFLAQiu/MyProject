using System;
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
        public override List<string> GetImgs(string url, object appendAssist = null) {
            if (url.IsNullOrWhiteSpace()) return null;
            string handerType = "";
            if (appendAssist != null) handerType = (string)appendAssist;
            switch (handerType) {
                case "detail": return DetailHander(url);//详情图片
            }
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
        /// <summary>
        /// 获取详情图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<string> DetailHander(string url) {
            //获取请求结果字符串
            var resultStr = CommonHelper.DoGetRequest(url);
            if(resultStr.IsNullOrWhiteSpace())return null;
            //获取详细信息数据URL
            var descURL = GetDescDetailURL(resultStr);
            if (descURL.IsNullOrWhiteSpace()) return null;
            //请求详情url获取详情数据字符串
            var resultDescStr=CommonHelper.DoGetRequest(descURL);
            if(resultDescStr.IsNullOrWhiteSpace())return null;
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
            Regex regex = new Regex(@"<img[^>]*src=""([^""]*)"">");
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
