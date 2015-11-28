using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using Core.Domain;
using Core.Domain.dns;
using StructureMap;
using StructureMap.Attributes;
using Sevices;

namespace Portal.App.DataSources
{
    public class DnsinstallationDataSource
    {
        public DnsinstallationDataSource()
        {
            ObjectFactory.BuildUp(this);            
        }

        public virtual string DefaultSortExpression
        {
            get { return "Id"; }
        }

        [SetterProperty]
        public IScriptGeneratorService<Dnsinstallation> Service { get; set; }

        public virtual IEnumerable<Dnsinstallation> SelectAll(
            string filterExpression, string startDate, string endDate, string sortExpression, int startRowIndex, int maximumRows)
        {
            var page = 0;
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

            IQueryable<Dnsinstallation> queryable;

            IEnumerable<Dnsinstallation> logs;

            if (!string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate))
            {
                var startTime = DateTime.ParseExact(startDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                var endTime = DateTime.ParseExact(endDate + " 23:59:59", "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                queryable = this.Service.Query().Where(x => x.CreatedDate >= startTime && x.CreatedDate <= endTime);
            }
            else
                queryable = this.Service.Query();

            if (string.IsNullOrWhiteSpace(sortExpression))
            {
                logs = queryable.Where(filterExpression).Skip(page * maximumRows).Take(
                        maximumRows);
            }
            else
            {
                if (!isDescending)
                {
                    logs = queryable.Where(filterExpression).OrderBy(sortExpression).Skip(
                            page * maximumRows).Take(maximumRows);
                }
                else
                {
                    logs = queryable.Where(filterExpression).OrderBy(sortExpression + " descending").Skip(page * maximumRows).Take(maximumRows);
                }
            }

            var result = logs.ToList();

            return result;
        }

        public virtual int SelectAllCount(string filterExpression, string startDate, string endDate)
        {
            if (string.IsNullOrWhiteSpace(filterExpression))
            {
                filterExpression = "1=1"; // create dummy where expression
            }

            var logs = this.Service.Query().Where(filterExpression);
            if (!string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate))
            {
                var startTime = DateTime.ParseExact(startDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                var endTime = DateTime.ParseExact(endDate + " 23:59:59", "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                logs = logs.Where(x => x.CreatedDate >= startTime && x.CreatedDate <= endTime);
            }
            return logs.Count();
        }

        public virtual void Delete(Dnsinstallation entity)
        {
            this.Service.Delete(entity);
        }
    }
}