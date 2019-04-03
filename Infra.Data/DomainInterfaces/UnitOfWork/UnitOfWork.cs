using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Infrastructure.DomainInterfaces.UnitOfWork
{
    public class EfUnitOfWork : UnitOfWorkBase
    {
        public EfUnitOfWork()
        {

        }
        public TransactionScope CurrentTransaction { get; set; }
        public override void Begin(UnitOfWorkOption options)
        {
            if (options.IsTransactional == true)
            {
                var transactionOption = new TransactionOptions
                {
                    IsolationLevel = options.IsolationLevel.GetValueOrDefault(IsolationLevel.ReadUncommitted)
                };
                if (options.TimeOut.HasValue) transactionOption.Timeout = options.TimeOut.Value;

                CurrentTransaction = new TransactionScope(options.ScopeOption.GetValueOrDefault(TransactionScopeOption.Required), transactionOption, options.AsyncFlowOption.GetValueOrDefault(TransactionScopeAsyncFlowOption.Enabled));
            }
        }

        public override void Commit()
        {
            SaveChanges();
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Complete();
            }
        }

        public Task CommitAsync()
        {
            throw new NotImplementedException();
        }


        public override void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
