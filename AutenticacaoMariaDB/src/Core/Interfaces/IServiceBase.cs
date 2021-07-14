using Infraestrutura.Data.Models;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IServiceBase<TEntity> where TEntity : class
    {
        List<ModelErrors> ValidateModel(TEntity entity);
    }
}
