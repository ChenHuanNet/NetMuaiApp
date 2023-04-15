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

        public async Task<List<DilidiliPCSourceItem>> AnalysisAsync(long sourceId, string currentSource, string mediaId, string detailId)
        {
            var dilidiliPCSourceItems = await _dilidiliPCSourceItemRepository.FindBySourceIdAsync(sourceId);
            if (dilidiliPCSourceItems != null && dilidiliPCSourceItems.Count > 0)
            {
                return dilidiliPCSourceItems;
            }

            dilidiliPCSourceItems = new List<DilidiliPCSourceItem>();

            string prefix = $"{AppConfigHelper.DiliDiliSourceHost}/play/{mediaId}/";

            var html = await _dilidiliSourceApi.GetSourceItemHtml(mediaId, detailId);

            string title = html.GetFirstHtmlWithAttr("div", "class", "title").RemoveHtmlLabel().Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();

            var sourcesHtml = html.GetFirstHtmlWithAttr("ul", "role", "tablist");
            var sources = sourcesHtml.GetHtmlWithAttr("li", "class", "nav-item");
            var contents = html.GetHtmlWithAttr("div", "role", "tabpanel");


            DilidiliPCSourceItem dilidiliPCSourceItem = null;
            for (int i = 0; i < sources.Count; i++)
            {
                string playLine = sources[i].Trim().GetHtmlLabel("a").RemoveHtmlLabel().Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                var lines = playLine.Split('(');
                string playSource = lines[0];
                if (playSource != currentSource)
                {
                    //只处理和当前进来的线路一样的视频
                    continue;
                }
                int maxNum = 0;
                if (lines.Length > 0)
                {
                    int.TryParse(playLine.Split('(')[1].Replace(")", ""), out maxNum);
                }

                List<HtmlALabel> list = new List<HtmlALabel>();
                if (contents.Count > i)
                {
                    list = contents[i].GetAHrefAndText();
                    list = list.Where(x => x.Href.Contains(prefix)).ToList();
                }
                foreach (var item in list)
                {
                    dilidiliPCSourceItem = await FindByUrlAsync(item.Href);
                    if (dilidiliPCSourceItem == null)
                    {
                        dilidiliPCSourceItem = new DilidiliPCSourceItem()
                        {
                            Id = 0,
                            Name = item.Text,
                            Sort = Convert.ToInt32(item.Text.Replace("第", "").Replace("集", "")),
                            SourceId = sourceId,
                            Url = item.Href
                        };
                    }
                    else
                    {
                        dilidiliPCSourceItem.Name = item.Text;
                        dilidiliPCSourceItem.Sort = Convert.ToInt32(item.Text.Replace("第", "").Replace("集", ""));
                        dilidiliPCSourceItem.SourceId = sourceId;
                        dilidiliPCSourceItem.Url = item.Href;
                    }

                    dilidiliPCSourceItems.Add(dilidiliPCSourceItem);
                }
            }

            return dilidiliPCSourceItems;
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


        public async Task<List<string>> DownloadVideos(long sourceId, List<string> urls)
        {
            List<string> localFiles = new List<string>();
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

                using (HttpClient httpClient = new HttpClient())
                {
                    var res = await httpClient.GetAsync(url);
                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stream = await res.Content.ReadAsStreamAsync();
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
                        stream.Close();

                        localFiles.Add(filePath);
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
    }
}
