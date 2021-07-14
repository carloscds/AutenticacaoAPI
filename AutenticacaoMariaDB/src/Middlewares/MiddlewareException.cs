using Infraestrutura.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Middlewares
{
    public class AutenticacaoAPIMiddlewareException
    {
        private readonly RequestDelegate _next;

        public AutenticacaoAPIMiddlewareException(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError;

            var _modelErros = new List<ModelErrors>();
            MontaMensagemErro(_modelErros, ex);

            var result = System.Text.Json.JsonSerializer.Serialize(_modelErros);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
        private static void MontaMensagemErro(List<ModelErrors> _modelErros, Exception ex)
        {
            if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            {
                _modelErros.Clear();
                _modelErros.Add(new ModelErrors { Campo = "Erro", Mensagem = "Deleção não permitida, objeto possui dependencia" });
                return;
            }
            _modelErros.Add(new ModelErrors { Campo = "Erro", Mensagem = ex.Message });
            if (ex.InnerException != null)
            {
                MontaMensagemErro(_modelErros, ex.InnerException);
            }
        }
    }
}
