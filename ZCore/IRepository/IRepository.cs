using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ZCore.Entities;

namespace ZCore.IRepository
{
    public interface IRepository
    {

    }
    public interface IRepository<TEntity> : IRepository<TEntity, int>, IRepository where TEntity : class, IEntity<int>
    {

    }
    public interface IRepository<TEntity, TPrimaryKey> : IRepository where TEntity : class, IEntity<TPrimaryKey>
    {
        #region 新增
        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity"></param>
        void Insert(TEntity entity);

        /// <summary>
        /// 异步新增实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        /// 新增实体并返回主键
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TPrimaryKey InsertAndGetId(TEntity entity);

        /// <summary>
        /// 异步新增实体并返回主键
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);

        /// <summary>
        /// 新增或更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity InsertOrUpdate(TEntity entity);

        /// <summary>
        /// 异步新增或更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> InsertOrUpdateAsync(TEntity entity);

        /// <summary>
        /// 新增或更新实体并返回主键Id
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TPrimaryKey InsertOrUpdateGetId(TEntity entity);

        /// <summary>
        /// 异步新增或更新实体并返回主键Id
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TPrimaryKey> InsertOrUpdateGetIdAsny(TEntity entity);
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="Id">主键Id</param>
        void Delete(TPrimaryKey Id);
        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="predicate">lamda表达式</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 异步删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(TEntity entity);
        /// <summary>
        /// 异步根据Id删除实体
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task DeleteAsync(TPrimaryKey Id);
        /// <summary>
        /// 异步根据条件删除实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region 查询

        /// <summary>
        /// 根据条件返回实体对象，如果是没有找到则返回null
        /// </summary>
        /// <param name="predicate">lamda表达式</param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 根据主键Id返回实体对象，如果是没有找到则返回null
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        TEntity FirstOrDefault(TPrimaryKey id);

        /// <summary>
        /// 异步根据条件返回实体对象，如果是没有找到则返回null
        /// </summary>
        /// <param name="predicate">lamda表达式</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 根据条件返回实体对象，如果返回null或多个实体则异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 异步根据条件返回实体对象，如果返回null或多个实体则异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 异步根据主键Id返回实体对象，如果是没有找到则返回null
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);
        /// <summary>
        /// 根据主键获取实体对象
        /// </summary>
        /// <param name="Id">主键Id</param>
        /// <returns></returns>
        TEntity Get(TPrimaryKey Id);

        /// <summary>
        /// 根据主键异步获取实体对象
        /// </summary>
        /// <param name="Id">主键Id</param>
        /// <returns></returns>
        TEntity GetAsync(TPrimaryKey Id);

        /// <summary>
        /// 根据条件查询,返回IQueryable对象
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 根据条件查询,返回IQueryable对象
        /// </summary>
        /// <param name="propertySelectors"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);

        /// <summary>
        /// 查询所有实体
        /// </summary>
        /// <returns></returns>
        List<TEntity> GetAllList();

        /// <summary>
        /// 异步查询所有实体
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> GetAllListAsync();

        /// <summary>
        /// 带条件获取实体列表
        /// </summary>
        /// <param name="predicate">lamda表达式</param>
        /// <returns></returns>
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 带条件获取实体列表
        /// </summary>
        /// <param name="predicate">lamda表达式</param>
        /// <returns></returns>
        List<TEntity> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 返回所有条数
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// 根据条件返回所有条数
        /// </summary>
        /// <param name="predicate">lamda表达式</param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 异步返回所有条数
        /// </summary>
        /// <returns></returns>
        int CountAsync();
        /// <summary>
        /// 异步根据条件返回所有条数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int CountAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Update(TEntity entity);
        /// <summary>
        /// 根据Id，更新实体对象
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAction"></param>
        /// <returns></returns>
        TEntity Update(TPrimaryKey Id, Action<TEntity> updateAction);
        /// <summary>
        /// 异步更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);
        /// <summary>
        /// 异步根据Id，更新实体对象
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAction"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TPrimaryKey Id, Action<TEntity> updateAction);
        #endregion
    }
}
