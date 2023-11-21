using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Domain.Dto
{
    public class Dilidili9SearchDto
    {
        /// <summary>
        /// 详情地址
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string thumb { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 哪年的
        /// </summary>
        public string time { get; set; }
        public string catid { get; set; }
        /// <summary>
        /// 主演
        /// </summary>
        public string star { get; set; }
        /// <summary>
        /// 连载集数
        /// </summary>
        public string lianzaijs { get; set; }
        public string beizhu { get; set; }
        public string alias_full { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string area { get; set; }
    }
}
