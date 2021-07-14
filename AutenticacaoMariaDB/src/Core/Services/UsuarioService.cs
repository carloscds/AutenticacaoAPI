using Abstractions.Common;
using Core.Interfaces;
using Core.Validation;
using Dominio;
using Infraestrutura.Data.Repositories;

namespace Core.Services
{
    public class UsuarioService : ServiceBase<Usuario>, IUsuarioService
    {
        private readonly IRepositoryBase<Usuario> _usuario;

        public UsuarioService(IRepositoryBase<Usuario> usuario)
        {
            _usuario = usuario;
        }

        public Usuario GetByEmail(string email)
        {
            return _usuario.GetByPredicate(w => w.Email == email);
        }

        public bool GetByEmailPassword(string email, string senha)
        {
            var usuario = _usuario.GetByPredicate(w => w.Email == email);
            if(usuario == null)
            {
                return false;
            }
            return Crypto.CheckPassword(senha,usuario.Senha);
        }

        public bool Add(Usuario usuario)
        {
            return _usuario.Add(usuario);
        }
                
        public bool Update(Usuario usuario)
        {
            return _usuario.Update(usuario);
        }
        
        public bool Delete(Usuario usuario)
        {
            return _usuario.Delete(usuario);
        }
    }
}
