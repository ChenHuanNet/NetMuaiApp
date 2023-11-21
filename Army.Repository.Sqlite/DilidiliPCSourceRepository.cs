using Army.Domain.Models;
using Army.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Army.Repository.Sqlite
{
    [Inject(OptionsLifetime = ServiceLifetime.Singleton)]
    public class DilidiliPCSourceRepository : BaseRepository<DilidiliPCSource>
    {
        public async Task<List<DilidiliPCSource>> FindLikeNameAsync(string name)
        {
            return await base.Database.QueryAsync<DilidiliPCSource>($" select * from DilidiliPCSource where Name like '%{name}%' order by Id desc ");
        }
    }
}
