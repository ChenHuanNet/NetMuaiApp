using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Infrastructure.Web
{
    [Serializable]
    public class PageList<T>
    {
        public int? Total { get; set; }

        public string Cursor { get; set; }

        public System.Collections.Generic.List<T> List { get; set; }

        public PageList()
        {
        }

        public PageList(System.Collections.Generic.List<T> list, int total)
        {
            this.List = list;
            this.Total = new int?(total);
        }

        public PageList(System.Collections.Generic.List<T> list, long total)
        {
            this.List = list;
            this.Total = new int?((int)total);
        }
    }
}
