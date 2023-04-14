using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Domain.Models
{

    /// <summary>
    /// 输入didi的官网网址，去解析source-list 样式底下的地址
    /// </summary>
    public class DilidiliPCSource
    {
        [SQLite.PrimaryKey]
        public long Id { get; set; }

        public string Name { get; set; }

        public string PlaySource { get; set; }

        public string Url { get; set; }

        public string Remark { get; set; }
     
    }
}
