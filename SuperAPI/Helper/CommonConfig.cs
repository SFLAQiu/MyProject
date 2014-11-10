using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LG.Utility;
namespace Helper {
    public  class CommonConfig {
        /// <summary>
        /// 图片保存地址
        /// </summary>
        public const string SaveTaoBaoPathConfig = "ItemImg\\{0}\\{1}\\{2}\\{3}\\";//{0} taobao {1} detail|item {2}id {3}imgs|pack
        public const string TaoBaoUrlConfig = "/ItemImg/{0}/{1}/{2}/{3}/";
        public const string ZipSavePath = "ItemImg\\Zip\\{0}\\";//{0} 时间日期
        public const string ZipUrlPre = "/ItemImg/Zip/{0}/";
    }
}
