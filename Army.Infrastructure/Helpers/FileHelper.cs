using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Army.Infrastructure.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// 将ts文件合并成一个mp4,但是ts文件必须是数字按顺序
        /// </summary>
        /// <param name="tsDir"></param>
        /// <returns></returns>
        public static string MergeTsVideo(string tsDir)
        {
            string outPath = Path.Combine(tsDir, DateTime.Now.ToString("yyyyMMddHHmmss_fff.mp4"));
            using (FileStream reader = new FileStream(outPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {

                var files = Directory.GetFiles(tsDir, "*.ts");
                List<string> fileList = new List<string>(files);
                fileList.Sort(
                    (x, y) =>
                    {
                        int indexX = Convert.ToInt32(x.Remove(x.IndexOf(".ts")).Substring(x.LastIndexOf(@"\") + 1));
                        int indexY = Convert.ToInt32(y.Remove(y.IndexOf(".ts")).Substring(y.LastIndexOf(@"\") + 1));
                        if (indexX > indexY)
                            return 1;
                        else if (indexX < indexY)
                            return -1;
                        else
                            return 0;
                    });
                foreach (var item in fileList)
                {
                    using (FileStream readStream = new FileStream(item, FileMode.Open, FileAccess.Read))
                    {
                        readStream.CopyTo(reader);
                    }
                }

            }

            return outPath;
        }
    }
}
