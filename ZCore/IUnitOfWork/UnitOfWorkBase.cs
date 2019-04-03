using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZCore.IUnitOfWork
{
    public abstract class UnitOfWorkBase : IUnitOfWork, IDisposable
    {
        public UnitOfWorkBase(IUnitOfWorkOptions options)
        {
            Id = new Guid().ToString("N");
            DefaultOptions = options;
        }
        public virtual void Begin(UnitOfWorkOptions options)
        {
            this.Options = options;


            BeginUow();
        }

        public void Execute()
        {
            try
            {
                ExecuteUow();
                _success = true;
                OnExecuted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        public abstract void BeginUow();
        public abstract void ExecuteUow();

        public abstract void OnExecuted();

        public virtual void SaveChanges()
        {
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            IsDisposed = true;
            if (!_success)
            {
                OnFiled(this._exception);
            }
            DisposeUow();
            OnDisposed();
        }

        public abstract void DisposeUow();

        public abstract void OnDisposed();

        public virtual void OnFiled(Exception exception)
        {
            throw exception;
        }

        public bool IsDisposed { get; private set; }

        private bool _success { get; set; }

        public UnitOfWorkOptions Options { get; private set; }

        private Exception _exception;

        private IUnitOfWorkOptions DefaultOptions { get; set; }

        string IUnitOfWork.Id => throw new NotImplementedException();

        private readonly string Id;
    }
}
