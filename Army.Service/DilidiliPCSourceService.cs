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
using System.Net.Http;
using Army.Domain.Dto;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using Mapster;

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
            string dir = AppConfigHelper.VideoDir;
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }

        public async Task InsertAsync(DilidiliPCSource model)
        {
            await _dilidiliPCSourceRepository.InsertAsync(model);
        }

        public async Task<DilidiliPCSource> FindAsync(string name, string time)
        {
            return await _dilidiliPCSourceRepository.FindSingleAsync(x => x.Name == name && x.Time == time);
        }


        public async Task<DilidiliPCSource> FindByIdAsync(long id)
        {
            return await _dilidiliPCSourceRepository.FindSingleAsync(x => x.Id == id );
        }

        public async Task SaveAsync(List<DilidiliPCSource> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                //只针对新解析的进行更新覆盖操作
                if (item.Id == 0)
                {
                    var temp = await FindAsync(item.Name, item.Time);
                    if (temp != null)
                    {
                        item.Adapt(temp);
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

        public async Task SaveAsync(DilidiliPCSource item)
        {
            //只针对新解析的进行更新覆盖操作
            if (item.Id == 0)
            {
                var temp = await FindAsync(item.Name, item.Time);
                if (temp != null)
                {
                    item.Adapt(temp);
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


        public async Task<List<DilidiliPCSource>> AnalysisAsync(string title)
        {
            var res = await _dilidiliSourceApi.SearchVideoAsync(Dilidili9Const.Search, title);
            var searchResult = JsonSerializer.Deserialize<List<Dilidili9SearchDto>>(res);

            List<DilidiliPCSource> result = new List<DilidiliPCSource>();
            foreach (var item in searchResult)
            {
                result.Add(new DilidiliPCSource()
                {
                    CurrentMaxNum = item.lianzaijs,
                    CurrentMaxText = "连载至 " + item.lianzaijs,
                    Id = 0,
                    Name = item.title,
                    DetailUrl = item.url,
                    Time = item.time,
                    Area = item.area,
                    Star = item.star,
                    Remark = item.beizhu,
                    Img = item.thumb,
                });
            }
            return result;
        }
    }
}
