using Dominio;
using FluentValidation;

namespace Core.Validation
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome em branco");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email em branco");
        }
    }
}
