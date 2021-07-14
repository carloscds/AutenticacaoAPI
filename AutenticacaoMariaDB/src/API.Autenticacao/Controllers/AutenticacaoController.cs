using API.Autenticacao.Models;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Identity;
using NetDevPack.Security.JwtSigningCredentials.Interfaces;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Dominio;
using Abstractions.Controller;
using Abstractions.Common;

namespace API.Autenticacao.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutenticacaoController : AutenticacaoAPIControllerBase
    {
        private readonly ILogger<AutenticacaoController> _logger;
        private readonly IUsuarioService _usuarioService;
        private readonly IJsonWebKeySetService _jwksService;

        public AutenticacaoController(IUsuarioService usuarioService,
                                      IJsonWebKeySetService jwksService,
                                      ILogger<AutenticacaoController> logger)
        {
            _usuarioService = usuarioService;
            _jwksService = jwksService;
            _logger = logger;
        }

        [HttpPost("CriarUsuario")]
        public IActionResult CriarUsuario([FromBody] UsuarioCreate usuario)
        {
            if(usuario.Senha != usuario.ConfirmaSenha)
            {
                return BadRequest("Senhas não conferem");
            }
            var user = new Usuario
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                Senha = Crypto.Encryption(usuario.Senha)
            };

            var error = _usuarioService.ValidateModel(user);
            if (error != null)
            {
                return BadRequest(error);
            }
            if (_usuarioService.GetByEmail(usuario.Email) != null)
            {
                return BadRequest("Email já existe");
            }
            if(_usuarioService.Add(user))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("Login")]
        public IActionResult LoginUsuario([FromBody] UsuarioLogin login)
        {
            if(_usuarioService.GetByEmailPassword(login.Email,login.Senha))
            {
                return Ok(GenerateJWT(login.Email));
            }
            return BadRequest();
        }

        private AuthJwtResponse GenerateJWT(string email)
        {
            var usuario = _usuarioService.GetByEmail(email);
            var claims = new List<Claim>();
            var identityClaims = GetUserClaims(claims, usuario);
            var encodedToken = EncodeToken(identityClaims);
            return GetTokenResponse(encodedToken, usuario, claims);
        }

        private ClaimsIdentity GetUserClaims(ICollection<Claim> claims, Usuario user)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Key.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);
            return identityClaims;
        }

        private static AuthJwtResponse GetTokenResponse(string encodedToken, Usuario user, IEnumerable<Claim> claims)
        {
            return new AuthJwtResponse
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(1).TotalSeconds,
                UserData = new UserData
                {
                    Id = user.Key.ToString(),
                    Email = user.Email,
                    Claims = claims.Select(c => new UserClaims { Type = c.Type, Value = c.Value })
                }
            };
        }

        private string EncodeToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var currentIssuer = $"{ControllerContext.HttpContext.Request.Scheme}://{ControllerContext.HttpContext.Request.Host}";
            var key = _jwksService.GetCurrent();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = currentIssuer,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = key
            });
            return tokenHandler.WriteToken(token);
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
