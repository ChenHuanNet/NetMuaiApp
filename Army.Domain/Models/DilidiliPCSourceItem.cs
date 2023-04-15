using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Domain.Models
{

    //  <div class="player-box">
    //  <script>
    //  var $player = {"id":"977293","video_id":"14921","src":"https:\/\/c2.monidai.com\/20230221\/eA1uFSxu\/index.m3u8","url":"https:\/\/www.dilidili8.cc\/play\/14921\/977293.html","title":"\u7b2c19\u96c6","vip":"0"};
    //  </script>
    //  <iframe id="player" src="https://www.dilidili8.cc/public/video/bjm3u8.html" scrolling="no" frameborder="0" width="100%" height="100%" allowfullscreen="allowfullscreen" mozallowfullscreen="mozallowfullscreen" msallowfullscreen="msallowfullscreen" oallowfullscreen="oallowfullscreen" webkitallowfullscreen="webkitallowfullscreen"></iframe>
    //  </div>



    //  #EXTM3U
    //  #EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=1000000,RESOLUTION=1280x720
    //  /ppvod/99972EC4A20C72F68C96CD0550272BE4.m3u8

    /// <summary>
    /// 1、先找到 样式player-box
    /// 2、找到里面的script
    /// 3、找到里面的$player 的 src https://c2.monidai.com/20230221/eA1uFSxu/index.m3u8 请求它 得到一个m3u8的文件
    /// 4、请求 https://c2.monidai.com/ppvod/99972EC4A20C72F68C96CD0550272BE4.m3u8  可以得到一个ts文件,后续 请求ts文件就是视频
    /// </summary>
    public class DilidiliPCSourceItem
    {
        [SQLite.PrimaryKey]
        public long Id { get; set; }
        public long SourceId { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
        public string DownloadText { get; set; }
    }
}
