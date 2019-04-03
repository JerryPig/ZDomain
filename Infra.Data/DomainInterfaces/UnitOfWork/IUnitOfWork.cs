using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DomainInterfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Begin(UnitOfWorkOption option);
        void Commit();

        //Task CommitAsync();

        void SaveChanges();

        //Task SaveChangesAsync();


    }
}
