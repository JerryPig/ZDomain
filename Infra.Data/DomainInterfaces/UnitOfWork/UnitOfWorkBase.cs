using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DomainInterfaces.UnitOfWork
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        public virtual void Begin(UnitOfWorkOption option)
        {

        }

        public virtual void Commit()
        {
            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public virtual void SaveChanges()
        {
        }

    }
}
