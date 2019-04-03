using System;
using System.Collections.Generic;
using System.Text;

namespace ZCore.Entities
{
    public interface IEntity
    {
    }

    public interface IEntity<TPrimaryKey> : IEntity
    {
        TPrimaryKey Id { get; set; }
    }
}
