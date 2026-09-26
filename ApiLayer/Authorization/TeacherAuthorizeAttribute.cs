using Microsoft.AspNetCore.Mvc;

namespace ApiLayer.Authorization
{
    public class TeacherAuthorizeAttribute : TypeFilterAttribute
    {
        public TeacherAuthorizeAttribute() : base(typeof(TeacherAuthorizationFilter))
        {
        }
    }
}