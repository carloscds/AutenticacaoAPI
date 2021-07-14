using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infraestrutura.Data.EF.Factory
{
    public class MigrationsContextFactoryBase : IDesignTimeDbContextFactory<APIDbContext>
    {
        public APIDbContext CreateDbContext(string[] args)
        {
            var diretorioPai = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var diretorioJson = Path.Combine(diretorioPai, @"API.Autenticacao");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(diretorioJson)
                .AddJsonFile("appsettings.json")
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<APIDbContext>();
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            optionsBuilder.UseMySql(configuration.GetConnectionString("BancoDados"), serverVersion,
                mysqlOptions => mysqlOptions.EnableRetryOnFailure());
            return new APIDbContext(optionsBuilder.Options);
        }
    }
}
