using Army.Domain.Consts;
using Army.Domain.Models;
using Army.Infrastructure.Models;
using Army.Repository.Sqlite;
using Army.Service.ApiClients;
using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Army.Infrastructure.Extensions;
using Army.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Http;
using Army.Domain.Dto;
using System.Threading;

namespace Army.Service
{
    [Inject(OptionsLifetime = ServiceLifetime.Singleton)]
    public class DilidiliPCSourceItemService
    {
        private readonly IdWorker _idWorker;
        private readonly DilidiliPCSourceItemRepository _dilidiliPCSourceItemRepository;
        private readonly IDilidiliSourceApi _dilidiliSourceApi;

        public DilidiliPCSourceItemService(IdWorker idWorker, DilidiliPCSourceItemRepository dilidiliPCSourceItemRepository, IDilidiliSourceApi dilidiliSourceApi)
        {
            _idWorker = idWorker;
            _dilidiliPCSourceItemRepository = dilidiliPCSourceItemRepository;
            _dilidiliSourceApi = dilidiliSourceApi;
        }

        public async Task<DilidiliPCSourceItem> FindAsync(long sourceId, string source, string num)
        {
            return await _dilidiliPCSourceItemRepository.FindSingleAsync(x => x.SourceId == sourceId && x.Source == source && x.Num == num);
        }

        public async Task DeleteByIdAsync(long id)
        {
            await _dilidiliPCSourceItemRepository.DeleteByIdAsync(id);
        }

        public async Task<string> AnalysisAsync(string html, string sourceVal)
        {
            html = html.Replace("\\", "").Replace("&amp;", "&");
            string id = "\"playiframe\"";
            if (html.Contains(id))
            {
                var start = html.IndexOf(id) + id.Length;
                html = html.Substring(start);
                var src = "src=\"";
                start = html.IndexOf(src) + src.Length;
                html = html.Substring(start);
                var end = html.IndexOf("\"");
                html = html.Substring(0, end);

                var ybUrl = html.Split('&').Where(x => x.Contains("yb_url=")).FirstOrDefault();
                ybUrl = ybUrl.Split('=')[1];
                ybUrl = ybUrl.Replace(".com497", ".com");

                return ybUrl;

                //var list = html.Split("|||");
                //foreach (var item in list)
                //{
                //    var arr = item.Split("$$$");
                //    var url = arr[0];
                //    var source = arr.Length > 1 ? arr[1] : "";
                //    if (string.IsNullOrWhiteSpace(source))
                //    {
                //        //第一个是没有$$$的
                //        var p = url.Split("&").Where(x => x.Contains("all_yb=")).FirstOrDefault();
                //        source = p.Split("=")[1];
                //        if (source == sourceVal)
                //        {
                //            return url;
                //        }
                //    }
                //    else
                //    {
                //        if (source == sourceVal)
                //        {
                //            return url;
                //        }
                //    }
                //}
            }
            return string.Empty;
        }


        public async Task<List<string>> GetTsVideos(string mu38)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage resp = null;
                var uri = new Uri(mu38);
                var host = uri.Scheme + "://" + uri.Authority;
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        string new_mu38 = mu38 + "?_t=" + new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                        resp = await httpClient.GetAsync(uri);
                        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            if (i == 4)
                            {
                                return new List<string>();
                            }
                            continue;
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 4)
                        {
                            return new List<string>();
                        }
                        continue;
                    }
                }

                var result = await resp.Content.ReadAsStringAsync();
                var line = result.Split(new char[] { '\r', '\n' }).Where(x => !x.StartsWith("#")).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(line))
                {
                    return new List<string>();
                }

                string tsHost = mu38.Substring(0, mu38.LastIndexOf("/") + 1);
                if (!line.StartsWith("http"))
                {
                    string tsUrl = "";
                    if (line.StartsWith("/"))
                    {
                        tsUrl = host + line;
                    }
                    else
                    {
                        tsUrl = tsHost + line;
                    }

                    var resp2 = await httpClient.GetAsync(tsUrl);
                    if (resp2.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return new List<string>();
                    }

                    result = await resp2.Content.ReadAsStringAsync();
                    var lines = result.Split(new char[] { '\r', '\n' }).Where(x => !x.StartsWith("#")).Select(x =>
                    {
                        if (x.StartsWith("http"))
                        {
                            return x;
                        }
                        else if (x.StartsWith("/"))
                        {
                            return host + x;
                        }
                        else
                        {
                            return tsHost + x;
                        }
                    }).ToList();

                    return lines;
                }
                else
                {
                    //直接就是ts了
                    var lines = result.Split(new char[] { '\r', '\n' }).Where(x => !x.StartsWith("#")).Select(x =>
                    {
                        if (x.StartsWith("http"))
                        {
                            return x;
                        }
                        else if (x.StartsWith("/"))
                        {
                            return host + x;
                        }
                        else
                        {
                            return tsHost + x;
                        }
                    }).ToList();
                    return lines;
                }
            }
        }


        public async Task SaveAsync(List<DilidiliPCSourceItem> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                //只针对新解析的进行更新覆盖操作
                if (item.Id == 0)
                {
                    var temp = await FindAsync(item.SourceId, item.Source, item.Num);
                    if (temp != null)
                    {
                        temp.Url = item.Url;
                        temp.Sort = item.Sort;
                        temp.Name = item.Name;
                        temp.Remark = item.Remark;
                        temp.DownloadText = item.DownloadText;
                        await _dilidiliPCSourceItemRepository.UpdateOneAsync(temp);
                    }
                    else
                    {
                        item.Id = _idWorker.NextId();
                        await _dilidiliPCSourceItemRepository.InsertAsync(item);
                    }
                }
                else
                {
                    await _dilidiliPCSourceItemRepository.UpdateOneAsync(item);
                }
            }
        }


        public async Task SaveAsync(DilidiliPCSourceItem item)
        {
            //只针对新解析的进行更新覆盖操作
            if (item.Id == 0)
            {
                var temp = await FindAsync(item.SourceId, item.Source, item.Num);
                if (temp != null)
                {
                    temp.Url = item.Url;
                    temp.Sort = item.Sort;
                    temp.Name = item.Name;
                    temp.Remark = item.Remark;
                    temp.DownloadText = item.DownloadText;
                    await _dilidiliPCSourceItemRepository.UpdateOneAsync(temp);
                }
                else
                {
                    item.Id = _idWorker.NextId();
                    await _dilidiliPCSourceItemRepository.InsertAsync(item);
                }
            }
            else
            {
                await _dilidiliPCSourceItemRepository.UpdateOneAsync(item);
            }
        }


        public async Task<List<string>> DownloadVideos(long sourceId, string source, string num, List<string> urls, Action<int, int> action)
        {
            List<string> localFiles = new List<string>();

            using HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10);

            foreach (var url in urls)
            {
                Uri uri = new Uri(url);

                string fileName = uri.AbsolutePath.TrimStart('/');

                string name = fileName.Substring(fileName.LastIndexOf('/'), fileName.Length - fileName.LastIndexOf('/')).TrimStart('/');

                string dir = Path.Combine(AppConfigHelper.VideoDir, sourceId.ToString(), source, num);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string filePath = Path.Combine(dir, name);

                if (File.Exists(filePath))
                {
                    //之前下载过的跳过
                    localFiles.Add(filePath);
                    action.Invoke(localFiles.Count, urls.Count);
                    continue;
                }

                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        var res = await httpClient.GetAsync(url);
                        if (res.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            using var stream = await res.Content.ReadAsStreamAsync();
                            //创建本地文件写入流
                            using (Stream fs = new FileStream(filePath, FileMode.Create))
                            {
                                byte[] bArr = new byte[1024];
                                int size = stream.Read(bArr, 0, bArr.Length);
                                while (size > 0)
                                {
                                    fs.Write(bArr, 0, size);
                                    size = stream.Read(bArr, 0, bArr.Length);
                                }
                            }

                            localFiles.Add(filePath);

                            action.Invoke(localFiles.Count, urls.Count);

                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(3000);
                        if (i == 4)
                        {
                            throw ex;
                        }
                    }
                }

            }



            return localFiles;
        }

        public void ClearFileCache(long sourceId)
        {
            string dir = Path.Combine(AppConfigHelper.VideoDir, sourceId.ToString());
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }

        public void ClearAllFileCache()
        {
            string dir = Path.Combine(AppConfigHelper.VideoDir);
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }


        public string MergeTsVideo(long sourceId, string source, string num, List<string> tsFiles, Action<int, int> action)
        {
            string dir = Path.Combine(AppConfigHelper.VideoDir, sourceId.ToString(), source, num);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string outPath = Path.Combine(dir, $"{sourceId}_{source}_{num}.mp4");
            using (FileStream reader = new FileStream(outPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                int i = 0;
                foreach (var item in tsFiles)
                {
                    using (FileStream readStream = new FileStream(item, FileMode.Open, FileAccess.Read))
                    {
                        readStream.CopyTo(reader);
                    }
                    i++;
                    action.Invoke(i, tsFiles.Count);

                    try
                    {
                        if (File.Exists(item))
                        {
                            File.Delete(item);
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                }
            }
            return outPath;
        }
    }
}
