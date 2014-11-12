using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LG.Utility;
namespace Helper {
    public class CommonConfig {
        /// <summary>
        /// 图片保存地址
        /// </summary>
        public const string SaveTaoBaoPathConfig = "ItemImg\\{0}\\{1}\\{2}\\{3}\\";//{0} taobao {1} detail|item {2}id {3}imgs|pack
        /// <summary>
        /// 图片URL前缀
        /// </summary>
        public const string TaoBaoUrlConfig = "/ItemImg/{0}/{1}/{2}/{3}/";
        /// <summary>
        /// 商品详情压缩包路径
        /// </summary>
        public const string ZipSavePath = "ItemImg\\Zip\\{0}\\";//{0} 时间日期
        /// <summary>
        /// 商品详情压缩包URL前缀
        /// </summary>
        public const string ZipUrlPre = "/ItemImg/Zip/{0}/";
        /// <summary>
        /// 同步图片地址配置
        /// </summary>
        public const string SynthesisImgPath = "Content\\SynthesisImg\\";
        /// <summary>
        /// 商品详情合成图片本地地址
        /// </summary>
        public const string SynthesisImgSavePath = "ItemImg\\SynthesisImg\\{0}\\";//{0} 时间日期
        /// <summary>
        /// 商品详情合成图片URL地址
        /// </summary>
        public const string SynthesisImgDownUrlPre = "/Home/DownSynthesisImg/?img={0}";
        /// <summary>
        /// 视频保存地址
        /// </summary>
        public const string VideoSavePath = "ItemImg\\Video\\{0}\\";
        /// <summary>
        /// 视频下载地址
        /// </summary>
        public const string VideoDownUrlPre = "/ItemImg/Video/{0}/";
    }
}
