using Core.Interfaces;
using Infraestrutura.Data.Models;
using FluentValidation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Core.Validation
{
    public class ServiceBase<TEntity> : IServiceBase<TEntity> where TEntity : class
    {
        private static ConcurrentDictionary<string, IValidator> _validators = new ConcurrentDictionary<string, IValidator>();
        private readonly static object objLock = new object();
        public IValidator Validator { get; }

        public List<ModelErrors> ValidateModel(TEntity entity)
        {
            var context = new ValidationContext<TEntity>(entity);
            var result = CreateValidator().Validate(context);
            if (!result.IsValid)
            {
                return result.Errors.Select(e => new ModelErrors() { Campo = e.PropertyName, Mensagem = e.ErrorMessage }).ToList();
            }
            return default;
        }

        public IValidator CreateValidator()
        {
            var validatorType = "Core.Validation." + typeof(TEntity).Name + "Validator";
            if (_validators.TryGetValue(validatorType, out IValidator value))
            {
                return value;
            }
            else
            {
                lock (objLock)
                {
                    var validatorName = Type.GetType(validatorType);
                    if (validatorName == null) return null;
                    var newValidator = (IValidator)Activator.CreateInstance(validatorName);
                    _validators.TryAdd(validatorName.FullName, newValidator);
                    return newValidator;
                }
            }
        }
    }
}
