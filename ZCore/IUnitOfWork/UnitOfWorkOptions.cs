using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace ZCore.IUnitOfWork
{
    public class UnitOfWorkOptions : IUnitOfWorkOptions
    {
        public UnitOfWorkOptions()
        {
            this.IsTransactional = true;
            this.ScopeOption = TransactionScopeOption.Required;
            this.IsTransactionScopeAvailable = true;
        }
        public bool? IsTransactional { get; set; }

        public IsolationLevel? IsolationLevel { get; set; }

        public TransactionScopeOption? ScopeOption { get; set; }

        public TimeSpan? TimeOut { get; set; }

        public TransactionScopeAsyncFlowOption? AsyncFlowOption { get; set; }
        public bool IsTransactionScopeAvailable { get; set; }
    }
}
