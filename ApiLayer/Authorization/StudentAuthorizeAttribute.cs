using Microsoft.AspNetCore.Mvc;

namespace ApiLayer.Authorization
{
    public class StudentAuthorizeAttribute : TypeFilterAttribute
    {
        public StudentAuthorizeAttribute() : base(typeof(StudentAuthorizationFilter))
        {
        }
    }
}