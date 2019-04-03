using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DomainInterfaces
{
    public interface IEntity
    {
    }

    public interface IEntity<TPrimaryKey> : IEntity
    {
        TPrimaryKey Id { get; set; }
    }
}
