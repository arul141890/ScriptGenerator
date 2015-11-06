// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScriptGeneratorService.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The ScriptGeneratorService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Sevices
{
    public interface IScriptGeneratorService<T>
    {
        #region Public Methods and Operators

        int Count();

        void Create(T entity);

        void Delete(T entity);

        void Delete(object id);

        IQueryable<T> Query();

        T Retrieve(object id);

        List<T> SelectAll();

        List<T> SelectAllPaged(int page, int pageSize);

        void Update(T entity);

        #endregion
    }
}