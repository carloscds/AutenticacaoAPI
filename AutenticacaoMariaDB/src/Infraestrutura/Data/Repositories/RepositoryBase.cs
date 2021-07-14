using System;
using System.Linq;
using System.Linq.Expressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Dominio.Abstract;
using Infraestrutura.Data.EF;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infraestrutura.Data.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        private readonly APIDbContext _db;
        private IDbContextTransaction _transacao;

        public RepositoryBase(APIDbContext db)
        {
            _db = db;
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, object>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, bool tracking = false)
        {
            IQueryable<TEntity> data;

            if (orderBy == null)
            {
                data = _db.Set<TEntity>().AsQueryable();
            }
            else
            {
                data = _db.Set<TEntity>().OrderBy(orderBy).AsQueryable();
            }
            if (!tracking)
            {
                data = data.AsNoTracking();
            }
            if (includes != null)
            {
                data = includes(data);
            }
            return data;
        }

        public TEntity GetID(int id, bool include = false)
        {
            var data = _db.Set<TEntity>().Find(id);
            if (include && data != null)
            {
                foreach (var navigation in _db.Entry(data).Navigations)
                {
                    navigation.Load();
                }
            }
            return data;
        }

        public TEntity GetByPredicate(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, bool tracking = false)
        {
            var data = _db.Set<TEntity>().AsQueryable();
            if (!tracking)
            {
                data = data.AsNoTracking();
            }
            if (includes != null)
            {
                data = includes(data);
            }
            return data.FirstOrDefault(predicate);
        }

        public IQueryable<TEntity> GetListByPredicate(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, bool tracking = false)
        {
            var data = _db.Set<TEntity>().Where(predicate);
            if (!tracking)
            {
                data = data.AsNoTracking();
            }
            if (includes != null)
            {
                data = includes(data);
            }
            return data;
        }

        public IQueryable<TEntity> GetListByPredicateSkipTake(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, bool tracking = false, int skip = 0, int take = 0)
        {
            var data = _db.Set<TEntity>().Where(predicate).Skip(skip).Take(take);
            if (!tracking)
            {
                data = data.AsNoTracking();
            }
            if (includes != null)
            {
                data = includes(data);
            }
            return data;
        }

        public bool Add(TEntity entity, bool AutoSave = true)
        {
            _db.Add(entity);
            if (AutoSave)
            {
                Save();
            }
            return true;
        }

        public bool AddRange(TEntity[] entities, bool AutoSave = true)
        {
            _db.AddRange(entities);
            if (AutoSave)
            {
                Save();
            }
            return true;
        }
        public bool Update(TEntity entity)
        {
            _db.Attach(entity).State = EntityState.Modified;
            Save();
            return true;
        }

        public bool UpdateRange(TEntity[] entities)
        {
            _db.UpdateRange(entities);
            Save();
            return true;
        }

        public bool Delete(int id)
        {
            var obj = GetID(id);
            if (obj == null)
            {
                throw new Exception("Objeto não existe");
            }
            _db.Remove(obj);
            Save();
            return true;
        }

        public bool Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new Exception("Objeto não existe");
            }
            _db.Remove(entity);
            Save();
            return true;
        }

        public bool DeleteByPredicate(Expression<Func<TEntity, bool>> predicate)
        {
            var data = _db.Set<TEntity>().Where(predicate);
            _db.RemoveRange(data);
            Save();
            return true;
        }

        public void StartTransacion()
        {
            _transacao = _db.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            try
            {
                _transacao.Commit();
            }
            catch
            {
                _transacao.Rollback();
                throw;
            }
        }

        public void Save()
        {
            _db.SaveChanges(true);
        }

        public void DetachObject(TEntity obj)
        {
            _db.Entry(obj).State = EntityState.Detached;
        }

        public APIDbContext GetDBContext() => _db;
    }
}
