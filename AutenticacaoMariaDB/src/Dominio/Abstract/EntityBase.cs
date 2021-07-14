using System;

namespace Dominio.Abstract
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        protected EntityBase()
        {
            Key = Guid.NewGuid();
        }
    }
}
