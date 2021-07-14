using Dominio;

namespace Core.Interfaces
{
    public interface IUsuarioService : IServiceBase<Usuario>
    {
        bool Add(Usuario usuario);
        Usuario GetByEmail(string email);
        bool GetByEmailPassword(string email, string senha);
        bool Update(Usuario usuario);
        bool Delete(Usuario usuario);
    }
}
