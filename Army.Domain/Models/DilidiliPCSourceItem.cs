using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Domain.Models
{
    /// <summary>
    /// http://ss2.quelingfei.com:9900/f/dpcomp.php?vid=&m=-1&cp=1&dy=2&i=84628&pt=2&line=0&_qp_get=fs&sl=0&all_yb=lz|||https://v.cdnlz1.com497/20231107/23458_37208a6b/index.m3u8$$$ff|||https://vip.ffzyread1.com497/20231107/20580_3bdaedc5/index.m3u8$$$sn|||https://v.gsuus.com497/play/ZdPVmWna/index.m3u8$$$kb|||https://cdn19.vip-vip-yzzy.com497/20231107/7804_b7dc4af5/index.m3u8$$$fs|||https://s9.fsvod1.com497/20231107/WfsUFvC8/index.m3u8$$$uk|||https://ukzy.ukubf4.com497/20231107/NcTxRSEb/index.m3u8$$$hn|||https://hnzy.bfvvs.com497/play/YaO0l6Nb/index.m3u8$$$wj|||https://top.1080pzy.co/202311/07/DnRxxXtR073/video/index.m3u8&yb=1&yb_url=https://s9.fsvod1.com497/20231107/WfsUFvC8/index.m3u8&i4=300&ipad=0&is_al_p=0&oth=
    /// </summary>
    public class DilidiliPCSourceItem
    {
        [SQLite.PrimaryKey]
        public long Id { get; set; }
        public long SourceId { get; set; }
        public string Source { get; set; }
        public string Num { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
        /// <summary>
        /// 本地地址
        /// </summary>
        public string Url { get; set; }
        public string Remark { get; set; }
        public string DownloadText { get; set; }
    }
}
