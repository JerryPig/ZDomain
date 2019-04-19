using System;

namespace ZCore.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Begin(UnitOfWorkOptions option);
        void Execute();

        void Commit();

        string Id { get; }

    }
}
