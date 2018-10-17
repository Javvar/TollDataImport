/*
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Intertoll.TollDataImport.Data.DataContext
{
    public class BaseDataContext<T> : IDisposable where T:DbContext, new()
    {
        #region Fields

        protected T context { get; set; }

        #endregion

        public BaseDataContext()
        {
            context = new T();
        }

        #region Methods

        public void Save()
        {
            context.SaveChanges();
        }

        public void Refresh(RefreshMode _Mode, object Entity)
        {
            var OCAcontext = ((IObjectContextAdapter)context).ObjectContext;
            var refreshableObjects = (from entry in OCAcontext.ObjectStateManager.GetObjectStateEntries(
                                                       EntityState.Added
                                                       | EntityState.Deleted
                                                       | EntityState.Modified
                                                       | EntityState.Unchanged)
                                      where entry.EntityKey != null
                                      select entry.Entity).ToList();

            OCAcontext.Refresh(_Mode, refreshableObjects);
        }

        public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return context.Database.BeginTransaction(isolationLevel);
        }

        public DbContextTransaction BeginTransaction()
        {
            return context.Database.BeginTransaction();
        }

        #endregion

        #region IDisposable

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();

                    PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance);

                    foreach (PropertyInfo info in properties.Where(x => x.PropertyType == typeof(IDisposable)))
                    {
                        ((IDisposable)info.GetValue(this, null)).Dispose();
                    }
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
*/
