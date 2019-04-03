using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZCore.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Begin(UnitOfWorkOptions option);
        void Execute();

        void SaveChanges();

        string Id { get; }

    }
}
