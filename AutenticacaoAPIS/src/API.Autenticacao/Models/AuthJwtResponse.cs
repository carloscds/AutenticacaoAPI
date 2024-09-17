using System.Collections.Generic;

namespace API.Autenticacao.Models
{
    public class AuthJwtResponse
    {
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserData UserData { get; set; }
        public AppData AppData { get; set; }
    }

    public class UserData
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<UserClaims> Claims { get; set; }
    }

    public class AppData
    {
        public string Id { get; set; }
        public IEnumerable<UserClaims> Claims { get; set; }
    }

    public class UserClaims
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
