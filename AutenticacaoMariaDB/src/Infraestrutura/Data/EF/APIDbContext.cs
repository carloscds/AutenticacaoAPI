using Dominio;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.Jwt.Core.Model;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;

namespace Infraestrutura.Data.EF
{
    public class APIDbContext : IdentityDbContext<Usuario>, ISecurityKeyContext
    {
        public DbSet<KeyMaterial> SecurityKeys { get; set; }
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options) {}
        public DbSet<Usuario> Usuario { get; set; }
    }
}
