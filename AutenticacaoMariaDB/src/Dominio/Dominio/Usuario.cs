using Microsoft.AspNetCore.Identity;

namespace Dominio
{
    public class Usuario : IdentityUser
    {
        public string Nome { get; set; }
        public string Senha { get; set; }
        public bool Ativo {get; set;}
    }
}
