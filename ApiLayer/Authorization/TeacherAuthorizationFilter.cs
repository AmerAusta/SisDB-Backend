using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiLayer.Authorization
{
    public class TeacherAuthorizationFilter : IAsyncActionFilter
    {
        private readonly IAuthorizationService _authorizationService;

        public TeacherAuthorizationFilter(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            int teacherId = 0;

            // 1. محاولة قراءة teacherId أو id من ActionArguments
            var argument = context.ActionArguments
                .FirstOrDefault(kv => kv.Key.Equals("teacherId", StringComparison.OrdinalIgnoreCase) ||
                                      kv.Key.Equals("id", StringComparison.OrdinalIgnoreCase));

            if (argument.Value != null)
            {
                int.TryParse(argument.Value.ToString(), out teacherId);
            }

            // 2. إذا لم يجدها في Arguments، يقرؤها مباشرة من RouteValues
            if (teacherId <= 0 && context.RouteData.Values.TryGetValue("teacherId", out var routeVal))
            {
                int.TryParse(routeVal?.ToString(), out teacherId);
            }
            else if (teacherId <= 0 && context.RouteData.Values.TryGetValue("id", out var routeValId))
            {
                int.TryParse(routeValId?.ToString(), out teacherId);
            }

            // 3. تنفيذ فحص الصلاحية فقط إذا تم العثور على TeacherId صحيح
            if (teacherId > 0)
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    context.HttpContext.User, teacherId, "CanAccessTeacherData");

                // إذا فشل الـ Handler في التثبت من الملكية، يتم إرجاع 403
                if (!authResult.Succeeded)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            await next(); // متابعة الطلب في حال نجاح الفحص
        }
    }
}