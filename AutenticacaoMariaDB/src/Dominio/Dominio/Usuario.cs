using Dominio.Abstract;
using Microsoft.AspNetCore.Identity;

namespace Dominio
{
    public class Usuario : EntityBase
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
