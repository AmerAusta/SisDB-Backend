using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiLayer.Authorization
{
    public class StudentAuthorizationFilter : IAsyncActionFilter
    {
        private readonly IAuthorizationService _authorizationService;

        public StudentAuthorizationFilter(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            int studentId = 0;

            // 1. محاولة قراءة StudentId من ActionArguments
            var argument = context.ActionArguments
                .FirstOrDefault(kv => kv.Key.Equals("studentId", StringComparison.OrdinalIgnoreCase) ||
                                      kv.Key.Equals("id", StringComparison.OrdinalIgnoreCase));

            if (argument.Value != null)
            {
                int.TryParse(argument.Value.ToString(), out studentId);
            }

            // 2. إذا لم يجدها في Arguments، يقرؤها مباشرة من RouteValues
            if (studentId <= 0 && context.RouteData.Values.TryGetValue("StudentId", out var routeVal))
            {
                int.TryParse(routeVal?.ToString(), out studentId);
            }

            // 3. تنفيذ فحص الصلاحية فقط إذا تم العثور على StudentId صحيح
            if (studentId > 0)
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    context.HttpContext.User, studentId, "CanAccessStudentData");

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