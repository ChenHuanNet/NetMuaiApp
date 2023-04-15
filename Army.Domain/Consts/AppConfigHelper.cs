using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Army.Domain.Consts
{
    public class AppConfigHelper
    {
        public const string DiliDiliSourceHost = "https://www.dilidili8.cc";

        public readonly static string VideoDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "videos");
    }
}
