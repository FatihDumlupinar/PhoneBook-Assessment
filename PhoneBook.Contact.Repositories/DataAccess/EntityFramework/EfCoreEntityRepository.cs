using Microsoft.EntityFrameworkCore;
using PhoneBook.Contact.Entities.Db.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PhoneBook.Contact.Repositories.DataAccess.EntityFramework
{
    public class EfCoreEntityRepository<TEntity> : IEntityRepository<TEntity>
        where TEntity : BaseEntity, IEntity, new()
    {
        #region Ctor&Fields

        protected readonly DbSet<TEntity> _entities;
        protected readonly DbContext _dbContext;

        public EfCoreEntityRepository(DbContext dbContext)
        {
            _entities = dbContext.Set<TEntity>();
            _dbContext = dbContext;
        }

        #endregion

        #region Add&AddAll

        public void Add(TEntity entity)
        {
            _entities.Add(entity);
        }

        public void AddAll(IEnumerable<TEntity> entity)
        {
            _entities.AddRange(entity);
        }

        public async Task AddAllAsync(IEnumerable<TEntity> entity)
        {
            await _entities.AddRangeAsync(entity);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _entities.AddAsync(entity);
        }

        public async Task<TEntity> AddAsyncReturnEntity(TEntity entity)
        {
            await _entities.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> AddAllAsyncReturnEntities(IEnumerable<TEntity> entity)
        {
            await _entities.AddRangeAsync(entity);
            return entity;
        }

        #endregion

        #region Delete

        public void Delete(TEntity entity)
        {
            entity.IsActive = false;
            _entities.Update(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(() =>
            {
                entity.IsActive = false;
                _entities.Update(entity);
            });
        }

        #endregion

        #region Get&GetList

        public TEntity Get(Expression<Func<TEntity, bool>> filter = null)
        {
            var getData = _entities.AsNoTrackingWithIdentityResolution().Single(filter);
            return getData;
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var getData = await _entities.AsNoTrackingWithIdentityResolution().SingleAsync(filter);
            return getData;
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null)
        {
            var getData = _entities.AsNoTrackingWithIdentityResolution().Where(filter).ToList();
            return getData;
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var getData = await _entities.AsNoTrackingWithIdentityResolution().Where(filter).ToListAsync();
            return getData;
        }

        #endregion

        #region Update&UpdateAll

        public void Update(TEntity entity)
        {
            _entities.Update(entity);
        }

        public void UpdateAll(IEnumerable<TEntity> entity)
        {
            _entities.UpdateRange(entity);
        }

        public async Task UpdateAllAsync(IEnumerable<TEntity> entity)
        {
            await Task.Run(() =>
            {
                _entities.UpdateRange(entity);
            });
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() =>
            {
                _entities.Update(entity);
            });
        }

        #endregion

        #region SaveChange
        
        public bool SaveChange()
        {
            var count =_dbContext.SaveChanges();
            return count > 0;
        }

        public async Task<bool> SaveChangeAsync()
        {
            var count = await _dbContext.SaveChangesAsync();
            return count > 0;
        }

        #endregion

        #region Dispose
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        #endregion
    }
}
