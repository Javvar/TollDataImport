using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Intertoll.TollDataImport.DataRequest.Data.Model;

namespace Intertoll.TollDataImport.DataRequest.Data
{
    public abstract class BaseDataContext :  IDisposable
    {
        #region Fields

        protected abstract DbContext Context { get; }

        #endregion

        #region Methods

        public void Save()
        {
            var ChangedEntities = Context.ChangeTracker.Entries();

            foreach (var ChangedEntity in ChangedEntities.Where(x => x.Entity is IEntity))
            {
                var entity = (IEntity)ChangedEntity.Entity;

                switch (ChangedEntity.State)
                {
                    case EntityState.Added:
                        entity.OnBeforeInsert();
                        break;

                    case EntityState.Modified:
                        entity.OnBeforeUpdate();
                        break;
                }
            }

            Context.SaveChanges();
        }

        public void Refresh(RefreshMode _Mode, object Entity)
        {
            var _OCAcontext = ((IObjectContextAdapter)this).ObjectContext;
            var refreshableObjects = (from entry in _OCAcontext.ObjectStateManager.GetObjectStateEntries(
                                                       EntityState.Added
                                                       | EntityState.Deleted
                                                       | EntityState.Modified
                                                       | EntityState.Unchanged)
                                      where entry.EntityKey != null
                                      select entry.Entity).ToList();

            _OCAcontext.Refresh(_Mode, refreshableObjects);
        }

        public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return Context.Database.BeginTransaction(isolationLevel);
        }

        public DbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Context.Dispose();

                    PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance);

                    foreach (PropertyInfo info in properties.Where(x => x.PropertyType == typeof(IDisposable)))
                    {
                        ((IDisposable)info.GetValue(this, null)).Dispose();
                    }
                }
            }

            disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
