// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericDataSource.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The generic data source.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PaymentGateway.Portal.App.Code.DataSources
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using PaymentGateway.Services;
    using StructureMap;
    using StructureMap.Attributes;

    public class GenericDataSource<T>
        where T : class
    {

        public GenericDataSource()
        {
            ObjectFactory.BuildUp(this);
        }

        public virtual string DefaultSortExpression
        {
            get { return "Id"; }
        }

        [SetterProperty]
        public IPaymentGatewayService<T> Service { get; set; }

        public virtual void Delete(T entity)
        {
            this.Service.Delete(entity);
        }

        public virtual IEnumerable<T> SelectAll(
            string filterExpression, string sortExpression, int startRowIndex, int maximumRows)
        {
            int page = 0;
            if (startRowIndex != 0)
            {
                page = startRowIndex / maximumRows;
            }

            bool isDescending = sortExpression.Contains("DESC");
            if (isDescending)
            {
                sortExpression = sortExpression.Replace(" DESC", string.Empty);
            }

            if (string.IsNullOrWhiteSpace(sortExpression))
            {
                isDescending = true;
                sortExpression = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(filterExpression))
            {
                filterExpression = "1=1"; // create dummy where expression
            }

            List<T> logs;

            if (string.IsNullOrWhiteSpace(sortExpression))
            {
                logs =
                    this.Service.Query().Where(filterExpression).Skip(page * maximumRows).Take(
                        maximumRows).ToList();
            }
            else
            {
                if (!isDescending)
                {
                    logs =
                        this.Service.Query().Where(filterExpression).OrderBy(sortExpression).Skip(
                            page * maximumRows).Take(maximumRows).ToList();
                }
                else
                {
                    logs =
                        this.Service.Query().Where(filterExpression).OrderBy(sortExpression + " descending").Skip(page * maximumRows).Take(maximumRows).ToList();
                }
            }

            return logs;
        }

        public virtual int SelectAllCount(string filterExpression)
        {
            if (string.IsNullOrWhiteSpace(filterExpression))
            {
                filterExpression = "1=1"; // create dummy where expression
            }

            return this.Service.Query().Where(filterExpression).Count();
        }

        public virtual void Update(T entity)
        {
            this.Service.Update(entity);
        }
    }
}