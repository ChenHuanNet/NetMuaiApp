using Army.Domain.Models;
using Army.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Army.Repository.Sqlite
{
    [Inject(OptionsLifetime = ServiceLifetime.Singleton)]
    public class MyCollectionRepository : BaseRepository<MyCollection>
    {
        public async Task<MyCollection> FindBySourceIdAsync(long sourceId)
        {
            return await base.Database.FindWithQueryAsync<MyCollection>($" select * from MyCollection where SourceId = {sourceId} order by Id desc ");
        }
    }
}
