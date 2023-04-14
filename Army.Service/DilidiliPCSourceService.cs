using Army.Domain.Models;
using Army.Infrastructure.DependencyInjection;
using Army.Repository.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Army.Service
{
    [Inject(OptionsLifetime = ServiceLifetime.Singleton)]
    public class DilidiliPCSourceService
    {
        private readonly IdWorker _idWorker;
        private readonly DilidiliPCSourceRepository _dilidiliPCSourceRepository;

        public DilidiliPCSourceService(DilidiliPCSourceRepository dilidiliPCSourceRepository)
        {
            _dilidiliPCSourceRepository = dilidiliPCSourceRepository;
        }

        public async Task<List<DilidiliPCSource>> FindLikeNameAsync(string name)
        {
            return await _dilidiliPCSourceRepository.FindLikeNameAsync(name);
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
            foreach (var item in list)
            {
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
            }
        }
    }
}
