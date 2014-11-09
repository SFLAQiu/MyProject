using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LG.Utility;
namespace Web.Controllers {
    public class Base : Controller {
        /// <summary>
        /// 输出JSON字符串到页面中
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult WriteJson(object obj) {
            return Content(obj.GetJSON().GetJsonCallBackStr(), "text/json");
        }
    }
}