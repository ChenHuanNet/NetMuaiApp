
using Army.Domain.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Army.Repository.Sqlite
{
    public class BaseRepository<T> : IRepository<T> where T : class, new()
    {
        protected SQLiteAsyncConnection Database;

        public BaseRepository()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            Database.EnableWriteAheadLoggingAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        }


        #region 初始化


        public async Task ResetData()
        {
            await DropTable();
            await CreateTable();
        }

        private async Task DropTable()
        {
            await Database.DropTableAsync<DilidiliPCSource>();
            await Database.DropTableAsync<DilidiliPCSourceItem>();
            await Database.DropTableAsync<MyCollection>();
        }



        private async Task CreateTable()
        {

            await Database.CreateTableAsync<DilidiliPCSource>();
            await Database.CreateIndexAsync<DilidiliPCSource>(x => x.Name);


            await Database.CreateTableAsync<DilidiliPCSourceItem>();
            await Database.CreateIndexAsync<DilidiliPCSourceItem>(x => x.Name);
            await Database.CreateIndexAsync<DilidiliPCSourceItem>(x => x.SourceId);
            await Database.CreateIndexAsync<DilidiliPCSourceItem>(x => x.Source);
            await Database.CreateIndexAsync<DilidiliPCSourceItem>(x => x.Num);


            await Database.CreateTableAsync<MyCollection>();

        }

        #endregion

        #region 基础支持的Sql操作

        public async Task DeleteByIdAsync(object id)
        {
            await Database.DeleteAsync<T>(id);
        }

        public async Task<T> FindByIdAsync(object id, bool throwIfNotExist = true)
        {
            if (throwIfNotExist)
                return await Database.GetAsync<T>(id);
            else
                return await Database.FindAsync<T>(id);
        }

        public async Task<T> FindSingleAsync(Expression<Func<T, bool>> filter)
        {
            return await Database.FindAsync<T>(filter);
        }

        public async Task InsertAsync(T entity)
        {
            await Database.InsertAsync(entity);
        }

        public async Task<int> InsertManyAsync(T[] entities)
        {
            return await Database.InsertAllAsync(entities);
        }


        public async Task UpdateManyAsync(T[] entities)
        {
            await Database.UpdateAllAsync(entities);
        }

        public async Task<int> UpdateOneAsync(T entity)
        {
            return await Database.UpdateAsync(entity);
        }

        #endregion

    }
}
