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
    public class DilidiliPCSourceItemRepository : BaseRepository<DilidiliPCSourceItem>
    {
        public async Task<List<DilidiliPCSourceItem>> FindBySourceIdAsync(long sourceId)
        {
            await base.Init();
            return await base.Database.QueryAsync<DilidiliPCSourceItem>($" select * from DilidiliPCSourceItem where SourceId={sourceId} order by Sort desc ");
        }
    }
}
