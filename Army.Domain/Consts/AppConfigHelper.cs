using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Army.Domain.Consts
{
    public class AppConfigHelper
    {
        public const string DiliDiliSourceHost = "http://dilidili9.com";

        public const string DiliDiliSourceHostEncoding = "%68%74%74%70%3a%2f%2f%64%69%6c%69%64%69%6c%69%39%2e%63%6f%6d";

        public readonly static string VideoDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "videos");
    }
}
