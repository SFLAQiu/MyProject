using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helper;
using LG.Utility;

namespace Web.Controllers
{
    public class HomeController : Base
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return Content("What");
        }
        /// <summary>
        /// 下载合成图片
        /// </summary>
        /// <returns></returns>
        public ActionResult DownSynthesisImg() {
            var fileName = Request.GetQ("img");
            if (fileName.IsNullOrWhiteSpace()) return WriteJson(new {Code="101",Msg="参数img不能为空！" });
            var filePath = HttpContext.Server.MapPath("~\\" + CommonConfig.SynthesisImgSavePath.FormatStr(DateTime.Now.Date.ToString("yyyy-MM-dd"))) + fileName;
            if (!new FileInfo(filePath).Exists) return WriteJson(new { Code = "101", Msg = "There is no picture！" });
            StaticFunctions.OutClientToDownFile(filePath, fileName);
            return Content("");
        }
    }
}
