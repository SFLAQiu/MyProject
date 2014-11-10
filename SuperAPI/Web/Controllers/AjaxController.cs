using System;
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
namespace Web.Controllers {
    public class AjaxController : Base {
        //
        // GET: /Ajax/

        public ActionResult Index() {
            return View();
        }
        /// <summary>
        /// 获取并下载商品图片
        /// </summary>
        /// <returns></returns>
        public ActionResult GetImgs() {
            //校验并获取域名对应的操作对象
            HostDo hander;
            Dictionary<string, string> values;
            var checkMsg = DoCheck(out hander, out values);
            if (checkMsg != null || hander == null) return checkMsg;
            //获取商品对应的保存地址
            var savePath = hander.GetSavePath(values["idStr"]);
            if (savePath.IsNullOrWhiteSpace()) return WriteJson(new {
                Code = "101",
                Msg = "无法解析商品图片保存地址！"
            });
            //获取保存地址的下载图片是否存在，如果存在就不再进行下载
            var rtnImgDatas = GetImgsByPath(savePath, hander.GetUrl(values["idStr"]));
            if (rtnImgDatas != null && rtnImgDatas.Count > 0) return WriteJson(new {
                Code = "100",
                Msg = "Bingo",
                Datas = rtnImgDatas
            });

            //不存在的话就进行分析，下载
            if (hander == null) return WriteJson(new {
                Code = "101",
                Msg = "无法匹配URL地址的操作！！"
            });

            var datas = hander.GetImgs(values["url"], values["action"]);
            if (datas == null || datas.Count <= 0) return WriteJson(new {
                Code = "101",
                Msg = "无法匹配地址的图片！！"
            });
            var i = 1;
            foreach (var imgUrl in datas) {
                CommonHelper.SaveFile(imgUrl, savePath, i);
                i++;
            }

            rtnImgDatas = GetImgsByPath(savePath, hander.GetUrl(values["idStr"]));

            return WriteJson(new {
                Code = "100",
                Msg = "Bingo",
                Datas = rtnImgDatas
            });
        }
        private ActionResult DoCheck(out HostDo hander, out Dictionary<string, string> values) {
            string url = Request.GetF("url");
            string type = Request.GetQ("type");
            string action = Request.GetQ("action");
            hander = null;
            values = null;
            if (url.IsNullOrWhiteSpace()) return WriteJson(new {
                Code = "501",
                Msg = "URL参数不能为空！"
            });
            string host, queryStr;
            try {
                var uri = new Uri(url);
                host = uri.Host;
                queryStr = uri.Query;
            } catch {
                return WriteJson(new {
                    Code = "101",
                    Msg = "非法URL！！"
                });
            }
            if (host.IsNullOrWhiteSpace() || queryStr.IsNullOrWhiteSpace()) return WriteJson(new {
                Code = "101",
                Msg = "非法URL！！"
            });
            var math = Regex.Match(queryStr, @"id=([\d]*[^&])");
            if (math == null || math.Groups.Count < 2) return WriteJson(new {
                Code = "101",
                Msg = "非法URL，解析不到商品ID！！"
            });
            var idStr = math.Groups[1].Value;
            if (idStr.IsNullOrWhiteSpace()) return WriteJson(new {
                Code = "101",
                Msg = "非法URL，解析不到商品ID！！"
            });
            values = new Dictionary<string, string> {
               {"url",url},
               {"type",type},
               {"action",action},
               {"idStr",idStr}
            };
            //获取域名对应的操作对象
            hander = Facroty.GetFacroty(host);
            return null;
        }

        public ActionResult DownLoadImgs() {
            //校验并获取域名对应的操作对象
            HostDo hander;
            Dictionary<string, string> values;
            var checkMsg = DoCheck(out hander, out values);
            if (checkMsg != null || hander == null) return checkMsg;
            string imgsStr = Request.GetF("imgs");
            var zipImgs = imgsStr.ToSplitList<string>(',');
            if (imgsStr.IsNullOrWhiteSpace() ||zipImgs == null || zipImgs.Count <= 0) return WriteJson(new {
                Code = "101",
                Msg = "需要下载哪些图？！" 
            });
            var fileName = (Guid.NewGuid() + DateTime.Now.GetTimestamp().ToString()).MD5() + ".zip";
            var saveZipFilePath=Server.MapPath("~\\"+CommonConfig.ZipSavePath.FormatStr(DateTime.Now.Date.ToString("yyy-MM-dd")))+fileName;
            var zipUrl=CommonConfig.ZipUrlPre.FormatStr(DateTime.Now.Date.ToString("yyy-MM-dd"))+fileName;
            var filePaths = new List<string>();
            //获取需要下载的文件集合
            foreach (var item in zipImgs) {
                if (item.Contains("?") || item.Contains("\\") || item.Contains("//") || item.Contains("#")) continue;
                string filePath = hander.GetSavePath(values["idStr"]) + item;
                if (!new FileInfo(filePath).Exists) continue;
                filePaths.Add(filePath);
            }
            //写入压缩包
            try {
                ZipHelper.CommpressDirByFiles(saveZipFilePath, filePaths.ToArray(), false, false);
            } catch (Exception ex) {
                return WriteJson(new { 
                    Code = "101",
                    Msg = ex.Message 
                });
            }
            return WriteJson(new {
                Code = "100", 
                Msg = "Bingo",
                DownUrl=zipUrl 
            });
        }


        #region "辅助"
        public List<string> GetImgsByPath(string path, string urlPre) {
            List<string> datas = new List<string>();
            if (!Directory.Exists(path)) return null;
            var files = Directory.GetFiles(path);
            if (files.Count() <= 0) return null;
            foreach (var item in files) {
                var fileInfo = new FileInfo(item);
                var fileName = fileInfo.Name;
                var fileUrl = urlPre + fileName;
                datas.Add(fileUrl);
            }
            return datas.OrderByDescending(d => d).ToList();
        }
        #endregion

    }
}
