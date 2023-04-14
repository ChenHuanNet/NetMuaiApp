using Army.Infrastructure.Web;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Army.Repository.Sqlite
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// 根据主键查询单个实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="throwIfNotExist"></param>
        /// <returns></returns>
        Task<T> FindByIdAsync(object id, bool throwIfNotExist = true);

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<T> FindSingleAsync(Expression<Func<T, bool>> filter);





        /// <summary>
        /// 新增
        /// </summary>
        Task InsertAsync(T entity);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entities"></param>
        Task<int> InsertManyAsync(T[] entities);


        /// <summary>
        /// 批量更新实体的所有属性
        /// </summary>
        Task UpdateManyAsync(T[] entitys);
        /// <summary>
        /// 更新一个实体的所有属性
        /// </summary>
        Task<int> UpdateOneAsync(T entity);



        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        Task DeleteByIdAsync(object id);



    }
}
