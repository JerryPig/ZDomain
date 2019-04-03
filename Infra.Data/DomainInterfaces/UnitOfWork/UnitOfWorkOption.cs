using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Infrastructure.DomainInterfaces.UnitOfWork
{
    public class UnitOfWorkOption
    {
        public UnitOfWorkOption()
        {
            this.IsTransactional = true;
            this.ScopeOption = TransactionScopeOption.Required;
        }
        public bool? IsTransactional { get; set; }

        public IsolationLevel? IsolationLevel { get; set; }

        public TransactionScopeOption? ScopeOption { get; set; }

        public TimeSpan? TimeOut { get; set; }

        public TransactionScopeAsyncFlowOption? AsyncFlowOption { get; set; }
    }
}
