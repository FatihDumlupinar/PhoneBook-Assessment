using PhoneBook.Contact.Entities.Db.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PhoneBook.Contact.Repositories.DataAccess
{
    public interface IEntityRepository<TEntity> : IDisposable where TEntity : BaseEntity, IEntity, new()
    {
        #region Get

        TEntity Get(Expression<Func<TEntity, bool>> filter = null);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null);

        #endregion

        #region GetList

        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null);

        #endregion

        #region Update

        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);

        #endregion

        #region UpdateAll

        void UpdateAll(IEnumerable<TEntity> entity);
        Task UpdateAllAsync(IEnumerable<TEntity> entity);

        #endregion

        #region Add

        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        Task<TEntity> AddAsyncReturnEntity(TEntity entity);

        #endregion

        #region AddAll

        void AddAll(IEnumerable<TEntity> entity);
        Task AddAllAsync(IEnumerable<TEntity> entity);
        Task<IEnumerable<TEntity>> AddAllAsyncReturnEntities(IEnumerable<TEntity> entity);

        #endregion

        #region Delete

        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);

        #endregion

        #region SaveChange

        bool SaveChange();
        Task<bool> SaveChangeAsync();

        #endregion

    }
}
