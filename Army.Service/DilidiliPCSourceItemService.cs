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
            string prefix = $"{AppConfigHelper.DiliDiliSourceHost}/play/{mediaId}/";

            var html = await _dilidiliSourceApi.GetSourceItemHtml(mediaId, detailId);

            string title = html.GetFirstHtmlWithAttr("div", "class", "title").RemoveHtmlLabel().Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();

            var sourcesHtml = html.GetFirstHtmlWithAttr("ul", "role", "tablist");
            var sources = sourcesHtml.GetHtmlWithAttr("li", "class", "nav-item");
            var contents = html.GetHtmlWithAttr("div", "role", "tabpanel");

            var dilidiliPCSourceItems = new List<DilidiliPCSourceItem>();
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
    }
}
