using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Domain.Models
{
    public class MyCollection
    {
        [SQLite.PrimaryKey]
        public long Id { get; set; }

        public long SourceId { get; set; }

        public string Name { get; set; }

        public string DetailUrl { get; set; }

        public string CurrentMaxNum { get; set; }

        public string CurrentMaxText { get; set; }

        public string Remark { get; set; }

        public string Star { get; set; }
        public string Area { get; set; }
        public string Time { get; set; }

        public string Img { get; set; }
    }
}
