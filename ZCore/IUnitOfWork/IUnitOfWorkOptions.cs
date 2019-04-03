using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace ZCore.IUnitOfWork
{
    public interface IUnitOfWorkOptions
    {
        bool? IsTransactional { get; set; }

        IsolationLevel? IsolationLevel { get; set; }

        TransactionScopeOption? ScopeOption { get; set; }

        TimeSpan? TimeOut { get; set; }

        TransactionScopeAsyncFlowOption? AsyncFlowOption { get; set; }

        bool IsTransactionScopeAvailable { get; set; }


    }
}
