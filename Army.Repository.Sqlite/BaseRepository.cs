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

        }


        #region 初始化

        protected async Task Init()
        {
            if (Database != null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            await Database.EnableWriteAheadLoggingAsync();
            await CreateTable();
        }

        public async Task ResetData()
        {
            await DropTable();
            await CreateTable();
        }

        private async Task DropTable()
        {
            if (await ExistTable<DilidiliPCSource>())
                await Database.DropTableAsync<DilidiliPCSource>();
            if (await ExistTable<DilidiliPCSourceItem>())
                await Database.DropTableAsync<DilidiliPCSourceItem>();
        }


        private async Task<bool> ExistTable<T1>()
        {
            string sql = $" SELECT name FROM sqlite_master WHERE type='table' AND name='{typeof(T1).Name}'; ";
            var num = await Database.ExecuteAsync(sql);
            return num > 0;
        }


        private async Task CreateTable()
        {
            await Database.CreateTableAsync<DilidiliPCSource>();
            await Database.CreateIndexAsync<DilidiliPCSource>(x => x.Id);
            await Database.CreateIndexAsync<DilidiliPCSource>(x => x.Name);


            await Database.CreateTableAsync<DilidiliPCSourceItem>();
            await Database.CreateIndexAsync<DilidiliPCSourceItem>(x => x.Id);
            await Database.CreateIndexAsync<DilidiliPCSourceItem>(x => x.Name);
            await Database.CreateIndexAsync<DilidiliPCSourceItem>(x => x.SourceId);
        }

        #endregion

        #region 基础支持的Sql操作

        public async Task DeleteByIdAsync(object id)
        {
            await Init();
            await Database.DeleteAsync<T>(id);
        }

        public async Task<T> FindByIdAsync(object id, bool throwIfNotExist = true)
        {
            await Init();
            if (throwIfNotExist)
                return await Database.GetAsync<T>(id);
            else
                return await Database.FindAsync<T>(id);
        }

        public async Task<T> FindSingleAsync(Expression<Func<T, bool>> filter)
        {
            await Init();
            return await Database.FindAsync<T>(filter);
        }

        public async Task InsertAsync(T entity)
        {
            await Init();
            await Database.InsertAsync(entity);
        }

        public async Task<int> InsertManyAsync(T[] entities)
        {
            await Init();
            return await Database.InsertAllAsync(entities);
        }


        public async Task UpdateManyAsync(T[] entities)
        {
            await Init();
            await Database.UpdateAllAsync(entities);
        }

        public async Task<int> UpdateOneAsync(T entity)
        {
            await Init();
            return await Database.UpdateAsync(entity);
        }

        #endregion

    }
}
