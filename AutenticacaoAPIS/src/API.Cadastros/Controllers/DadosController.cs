using Abstractions.Controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace API.Cadastros.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DadosController : AutenticacaoAPIControllerBase
    {
        private readonly ILogger<DadosController> _logger;

        public DadosController(ILogger<DadosController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok($"GET: {DateTime.Now.ToString()} - Usuario: {LoggedUser}");
        }
    }
}
