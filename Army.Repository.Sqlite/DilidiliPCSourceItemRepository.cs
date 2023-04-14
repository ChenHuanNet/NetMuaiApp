using Army.Domain.Models;
using Army.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Repository.Sqlite
{
    [Inject(OptionsLifetime = ServiceLifetime.Singleton)]
    public class DilidiliPCSourceItemRepository : BaseRepository<DilidiliPCSourceItem>
    {
    }
}
