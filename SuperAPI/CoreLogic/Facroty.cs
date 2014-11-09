using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LG.Utility;

namespace CoreLogic {
    public class Facroty {
        /// <summary>
        /// 获得操作对象
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static HostDo GetFacroty(string host) {
            if (!TaoBao.Host.FirstOrDefault(d =>host.Contains(d)).IsNullOrWhiteSpace()) return new TaoBao();
            return null;
        }
    }
}
