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
    public class MyCollectionService
    {

        private readonly IdWorker _idWorker;
        private readonly MyCollectionRepository _myCollectionRepository;

        public MyCollectionService(IdWorker idWorker, MyCollectionRepository myCollectionRepository)
        {
            _idWorker = idWorker;
            _myCollectionRepository = myCollectionRepository;
        }


        public async Task<MyCollection> FindBySourceIdAsync(long sourceId)
        {
           return await _myCollectionRepository.FindBySourceIdAsync(sourceId);
        }
    }
}
