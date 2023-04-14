using Army.Domain.Models;
using Army.Infrastructure.DependencyInjection;
using Army.Infrastructure.Models;
using Army.Repository.Sqlite;
using Army.Service.ApiClients;
using Microsoft.Extensions.DependencyInjection;
using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Army.Infrastructure.Extensions;
using Army.Domain.Consts;

namespace Army.Service
{
    [Inject(OptionsLifetime = ServiceLifetime.Singleton)]
    public class DilidiliPCSourceService
    {
        private readonly IdWorker _idWorker;
        private readonly DilidiliPCSourceRepository _dilidiliPCSourceRepository;
        private readonly IDilidiliSourceApi _dilidiliSourceApi;

        public DilidiliPCSourceService(DilidiliPCSourceRepository dilidiliPCSourceRepository, IDilidiliSourceApi dalidiliPCSourceApi, IdWorker idWorker)
        {
            _dilidiliPCSourceRepository = dilidiliPCSourceRepository;
            _dilidiliSourceApi = dalidiliPCSourceApi;
            _idWorker = idWorker;
        }

        public async Task<List<DilidiliPCSource>> FindLikeNameAsync(string name)
        {
            var data = await _dilidiliPCSourceRepository.FindLikeNameAsync(name);
            data = data.OrderByDescending(x => x.CurrentMaxNum).ToList();
            return data;
        }

        public async Task ResetDataAsync()
        {
            await _dilidiliPCSourceRepository.ResetData();
        }

        public async Task InsertAsync(DilidiliPCSource model)
        {
            await _dilidiliPCSourceRepository.InsertAsync(model);
        }

        public async Task<DilidiliPCSource> FindAsync(string name, string source)
        {
            return await _dilidiliPCSourceRepository.FindSingleAsync(x => x.Name == name && x.PlaySource == source);
        }

        public async Task SaveAsync(List<DilidiliPCSource> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                //只针对新解析的进行更新覆盖操作
                if (item.Id == 0)
                {
                    var temp = await FindAsync(item.Name, item.PlaySource);
                    if (temp != null)
                    {
                        temp.PlaySource = item.PlaySource;
                        temp.Url = item.Url;
                        temp.Remark = item.Remark;
                        temp.Name = item.Name;
                        await _dilidiliPCSourceRepository.UpdateOneAsync(temp);
                    }
                    else
                    {
                        item.Id = _idWorker.NextId();
                        await _dilidiliPCSourceRepository.InsertAsync(item);
                    }
                }
                else
                {
                    await _dilidiliPCSourceRepository.UpdateOneAsync(item);
                }
            }
        }


        public async Task<List<DilidiliPCSource>> AnalysisAsync(string mediaId)
        {
            string prefix = $"{AppConfigHelper.DiliDiliSourceHost}/play/{mediaId}/";

            var html = await _dilidiliSourceApi.GetSourceHtml(mediaId);

            string title = html.GetFirstHtmlWithAttr("div", "class", "title").RemoveHtmlLabel().Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();

            var sources = html.GetHtmlWithAttr("li", "class", "nav-item");
            var contents = html.GetHtmlWithAttr("div", "role", "tabpanel");

            var dilidiliPCSources = new List<DilidiliPCSource>();
            DilidiliPCSource dilidiliPCSource = null;
            for (int i = 0; i < sources.Count; i++)
            {

                string playLine = sources[i].Trim().GetHtmlLabel("a").RemoveHtmlLabel().Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                var lines = playLine.Split('(');
                string playSource = lines[0];
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
                dilidiliPCSource = await FindAsync(title, playSource);
                if (dilidiliPCSource == null)
                {
                    dilidiliPCSource = new DilidiliPCSource()
                    {
                        Id = 0,
                        Name = title,
                        PlaySource = playSource,
                        CurrentMaxNum = maxNum,
                        Url = list.First().Href
                    };
                }
                else
                {
                    dilidiliPCSource.CurrentMaxNum = maxNum;
                    dilidiliPCSource.Name = title;
                    dilidiliPCSource.PlaySource = playSource;
                    dilidiliPCSource.Url = list.First().Href;
                }


                dilidiliPCSources.Add(dilidiliPCSource);
            }

            return dilidiliPCSources;
        }
    }
}
