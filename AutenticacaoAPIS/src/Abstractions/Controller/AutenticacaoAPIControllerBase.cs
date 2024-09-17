using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Abstractions.Controller
{
    public class AutenticacaoAPIControllerBase : ControllerBase
    {
        public string LoggedUser
        {
            get
            {
                return HttpContext.User.Claims.FirstOrDefault(w => w.Type.Contains("nameidentifier")).Value;
            }
        }
    }
}
