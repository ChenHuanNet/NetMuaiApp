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

        public async Task<DilidiliPCSourceItem> FindByUrlAsync(string url)
        {
            return await _dilidiliPCSourceItemRepository.FindSingleAsync(x => x.Url == url);
        }

        public async Task AnalysisAsync(string detailId, int num, string qp)
        {
            var html = await _dilidiliSourceApi.GetVideoHtml(detailId, num, qp);

            html = html.GetFirstHtmlWithAttr("iframe", "id", "playiframe");
        }


        public async Task SaveAsync(List<DilidiliPCSourceItem> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                //只针对新解析的进行更新覆盖操作
                if (item.Id == 0)
                {
                    var temp = await FindByUrlAsync(item.Url);
                    if (temp != null)
                    {
                        temp.Url = item.Url;
                        temp.SourceId = item.SourceId;
                        temp.Sort = item.Sort;
                        temp.Name = item.Name;
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


        public async Task<List<string>> DownloadVideos(long sourceId, List<string> urls, Action<int, int> action)
        {
            List<string> localFiles = new List<string>();

            using HttpClient httpClient = new HttpClient();

            foreach (var url in urls)
            {
                Uri uri = new Uri(url);

                string fileName = uri.AbsolutePath.TrimStart('/');

                string name = fileName.Substring(fileName.LastIndexOf('/'), fileName.Length - fileName.LastIndexOf('/')).TrimStart('/');

                string dir = Path.Combine(AppConfigHelper.VideoDir, sourceId.ToString());
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string filePath = Path.Combine(dir, name);

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


        public string MergeTsVideo(long sourceId, List<string> tsFiles, Action<int, int> action)
        {
            string dir = Path.Combine(AppConfigHelper.VideoDir, sourceId.ToString());
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string outPath = Path.Combine(dir, sourceId + ".mp4");
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
                }
            }
            return outPath;
        }
    }
}
