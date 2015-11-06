// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptGeneratorService.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The payment gateway service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Core.Data;
using Data.UnitOfWorks;

namespace Sevices
{
    public class ScriptGeneratorService<T> : IScriptGeneratorService<T>
    {
        #region Fields

        protected readonly IScriptGeneratorUnitOfWork Uow;

        #endregion

        #region Constructors and Destructors

        public ScriptGeneratorService(IScriptGeneratorUnitOfWork uow)
        {
            this.Uow = uow;
        }

        #endregion

        #region Public Methods and Operators

        public int Count()
        {
            IRepository<T> repo = this.Uow.Repository<T>();
            return repo.Count();
        }

        public virtual void Create(T entity)
        {
            IRepository<T> repo = this.Uow.Repository<T>();
            repo.Create(entity);
            this.Uow.Commit();
        }

        public virtual void Delete(T entity)
        {
            IRepository<T> repo = this.Uow.Repository<T>();
            repo.Delete(entity);
            this.Uow.Commit();
        }

        public virtual void Delete(object id)
        {
            IRepository<T> repo = this.Uow.Repository<T>();
            repo.Delete(id);
            this.Uow.Commit();
        }

        public IQueryable<T> Query()
        {
            IRepository<T> repo = this.Uow.Repository<T>();
            return repo.Query();
        }

        public virtual T Retrieve(object id)
        {
            IRepository<T> repo = this.Uow.Repository<T>();
            return repo.Retrieve(id);
        }

        public virtual List<T> SelectAll()
        {
            IRepository<T> repo = this.Uow.Repository<T>();
            return repo.SelectAll().ToList();
        }

        public virtual List<T> SelectAllPaged(int page, int pageSize)
        {
            IRepository<T> repo = this.Uow.Repository<T>();
            return repo.SelectAllPaged(page, pageSize).ToList();
        }

        public virtual void Update(T entity)
        {
            IRepository<T> repo = this.Uow.Repository<T>();
            repo.Update(entity);
            this.Uow.Commit();
        }

        #endregion
    }
}