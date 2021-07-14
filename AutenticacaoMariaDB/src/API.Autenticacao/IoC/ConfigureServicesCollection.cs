using Core.Interfaces;
using Core.Services;
using Infraestrutura.Data.EF;
using Infraestrutura.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace API.Autenticacao.IoC
{
    public static class ConfigureServicesCollection
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            services.AddDbContext<APIDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("BancoDados"), serverVersion,
                mysqlOptions => mysqlOptions.EnableRetryOnFailure()));
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IUsuarioService, UsuarioService>();
        }
    }
}
