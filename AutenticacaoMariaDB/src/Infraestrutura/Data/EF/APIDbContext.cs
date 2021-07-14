using Dominio;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.JwtSigningCredentials;
using NetDevPack.Security.JwtSigningCredentials.Store.EntityFrameworkCore;

namespace Infraestrutura.Data.EF
{
    public class APIDbContext : DbContext, ISecurityKeyContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options) {}
        public DbSet<SecurityKeyWithPrivate> SecurityKeys { get; set; }
        public DbSet<Usuario> Usuario { get; set; }

    }
}
