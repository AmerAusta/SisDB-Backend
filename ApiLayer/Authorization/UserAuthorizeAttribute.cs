using Microsoft.AspNetCore.Mvc;

namespace ApiLayer.Authorization
{
    public class UserAuthorizeAttribute : TypeFilterAttribute
    {
        public UserAuthorizeAttribute() : base(typeof(UserAuthorizationFilter))
        {
        }
    }
}