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
using System.Drawing;
using System.Drawing.Imaging;
namespace Web.Controllers {
    public class AjaxController : Base {
        public ActionResult Index() {
            return Content(" What ？");
        }
        #region "商品图片"
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
            string msg;
            var datas = hander.GetImgs(values["url"], out msg);
            if (datas == null || datas.Count <= 0) return WriteJson(new {
                Code = "101",
                Msg = "无法匹配地址的图片！！"
            });
            var i = 1;
            foreach (var imgUrl in datas) {
                StaticFunctions.SaveFile(imgUrl, savePath, i.ToString());
                i++;
            }

            rtnImgDatas = GetImgsByPath(savePath, hander.GetUrl(values["idStr"]));

            return WriteJson(new {
                Code = "100",
                Msg = "Bingo",
                Datas = rtnImgDatas
            });
        }
        
        /// <summary>
        /// 写入压缩包
        /// </summary>
        /// <returns></returns>
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
        #endregion
        /// <summary>
        /// 合成图片
        /// </summary>
        /// <returns></returns>
        public ActionResult SynthesisImg() {
            //校验并获取域名对应的操作对象
            HostDo hander;
            Dictionary<string, string> values;
            var checkMsg = DoCheck(out hander, out values);
            if (checkMsg != null || hander == null) return checkMsg;
            string mainImg = Request.GetF("bImg");//背景图
            string sImg = Request.GetF("sImg");//合成图
            int xNum = Request.GetF("x").GetInt(0,false) ;
            int yNum = Request.GetF("y").GetInt(0, false);
            if (mainImg.IsNullOrWhiteSpace() || sImg.IsNullOrWhiteSpace()) return WriteJson(new { 
                Code="101",
                Msg="缺少参数"
            });
            var sImgFilePath = Server.MapPath("~\\" + CommonConfig.SynthesisImgPath) + sImg;
            var filePath = hander.GetSavePath(values["idStr"]) + mainImg;

            if (!new FileInfo(sImgFilePath).Exists) return WriteJson(new {
                Code = "101",
                Msg = "合成素材图不存在！"
            });
            if (!new FileInfo(filePath).Exists) return WriteJson(new {
                Code = "101",
                Msg = "背景图不存在！"
            });

            Bitmap b_map_bg = new Bitmap(filePath);
            Graphics gp_anise = Graphics.FromImage(b_map_bg);
            Point sImgPoint = new Point { X = xNum, Y = yNum };
            gp_anise.DrawImage(Image.FromFile(sImgFilePath), sImgPoint);
            
            byte[] imgData = null;
            using (MemoryStream ms = new MemoryStream()) {
                b_map_bg.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                imgData = new byte[ms.Length];
                ms.Read(imgData, 0, (int)ms.Length);
            }
            gp_anise.Dispose();
            b_map_bg.Dispose();

            var fileName = (Guid.NewGuid() + DateTime.Now.GetTimestamp().ToString()).MD5() + ".png";
            var path = HttpContext.Server.MapPath("~\\" + CommonConfig.SynthesisImgSavePath.FormatStr(DateTime.Now.Date.ToString("yyyy-MM-dd")));
            var synthesisImgfilePath = path + fileName;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            try {
                StaticFunctions.ByteToImg(imgData).Save(synthesisImgfilePath);
            }catch{
                return WriteJson(new {
                    Code = "101",
                    Msg = "生成图片失败！"
                });
            }
            return WriteJson(new {
                Code = "100",
                Msg = "Bingo",
                DownUrl = CommonConfig.SynthesisImgDownUrlPre.FormatStr(fileName)
            });
        }


        #region "辅助"
        /// <summary>
        /// 通过路径，获取路径下所有图片的url集合
        /// </summary>
        /// <param name="path"></param>
        /// <param name="urlPre"></param>
        /// <returns></returns>
        private List<string> GetImgsByPath(string path, string urlPre) {
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
            return datas.OrderBy(d => d).ToList();
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="hander"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private ActionResult DoCheck(out HostDo hander, out Dictionary<string, string> values) {
            string url = Request.GetF("url");
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
               {"idStr",idStr}
            };
            //获取域名对应的操作对象
            hander = Facroty.GetFacroty(host);
            return null;
        }
        #endregion

    }
}
