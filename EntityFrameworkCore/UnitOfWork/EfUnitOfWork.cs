using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ZCore.UnitOfWork;

namespace EntityFrameworkCore.UnitOfWork
{
    public class EfUnitOfWork : UnitOfWorkBase
    {


        public EfUnitOfWork()
        {
        }
        public override void BeginUow()
        {
            if (Options.IsTransactional == true)
            {
                var transactionOption = new TransactionOptions
                {
                    IsolationLevel = Options.IsolationLevel.GetValueOrDefault(IsolationLevel.ReadUncommitted)
                };
                if (Options.TimeOut.HasValue) transactionOption.Timeout = Options.TimeOut.Value;

                CurrentTransaction = new TransactionScope(Options.ScopeOption.GetValueOrDefault(TransactionScopeOption.Required), transactionOption, Options.AsyncFlowOption.GetValueOrDefault(TransactionScopeAsyncFlowOption.Enabled));
            }

        }

        public override void ExecuteUow()
        {
            SaveChanges();
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Complete();
            }
        }

        public TransactionScope CurrentTransaction { get; set; }

        public override void SaveChanges()
        {
            //foreach (var item in ActiveDbContexts.Values)
            //{
            //    SaveChangesInDbContext(item);
            //}
        }


        protected virtual void SaveChangesInDbContext(DbContext context)
        {
            context.SaveChanges();
        }

        public override void DisposeUow()
        {
            //foreach (var item in ActiveDbContexts.Values)
            //{
            //    item.Dispose();
            //}

            //ActiveDbContexts.Clear();
        }

        public override void OnExecuted()
        {
            throw new NotImplementedException();
        }

        public override void OnDisposed()
        {
            throw new NotImplementedException();
        }
    }
}
