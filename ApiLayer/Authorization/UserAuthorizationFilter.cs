using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ApiLayer.Authorization
{
    public class UserAuthorizationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            // 1. التجاوز التلقائي للـ SuperAdmin والـ Admin
            if (user.IsInRole("SuperAdmin") || user.IsInRole("Admin"))
            {
                await next();
                return;
            }

            int targetUserId = 0;

            // 2. قراءة UserId المطلوب من ActionArguments
            var argument = context.ActionArguments
                .FirstOrDefault(kv => kv.Key.Equals("userId", StringComparison.OrdinalIgnoreCase) ||
                                      kv.Key.Equals("id", StringComparison.OrdinalIgnoreCase));

            if (argument.Value != null)
            {
                int.TryParse(argument.Value.ToString(), out targetUserId);
            }

            // 3. قراءته من RouteValues إذا لم يوجد في Arguments
            if (targetUserId <= 0 && context.RouteData.Values.TryGetValue("UserId", out var routeVal))
            {
                int.TryParse(routeVal?.ToString(), out targetUserId);
            }

            // 4. قراءة UserId الخاص بالمستخدم صاحب التوكن
            var currentUserIdClaim = user.FindFirstValue("id")
                                  ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(currentUserIdClaim, out int currentUserId))
            {
                context.Result = new ForbidResult();
                return;
            }

            // 5. منع الوصول إذا كان المستخدم يحاول قراءة بيانات غيره
            if (targetUserId > 0 && targetUserId != currentUserId)
            {
                context.Result = new ForbidResult(); // 403 Forbidden
                return;
            }

            await next();
        }
    }
}