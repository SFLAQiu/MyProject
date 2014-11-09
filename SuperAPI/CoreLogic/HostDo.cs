using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLogic {
    public abstract class HostDo {
        /// <summary>
        /// 获取需要下载的图片集合
        /// </summary>
        /// <param name="appendAssist"></param>
        /// <returns></returns>
        public abstract List<string> GetImgs(string url, object appendAssist = null);
        /// <summary>
        /// 获取保存路径地址
        /// </summary>
        /// <param name="appendAssist"></param>
        /// <returns></returns>
        public abstract string GetSavePath(object appendAssist);
        /// <summary>
        /// 获取URL地址
        /// </summary>
        /// <param name="appendAssist"></param>
        /// <returns></returns>
        public abstract string GetUrl(object appendAssist);
    }
}
