using DataLayer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiLayer.Authorization
{
    public class TeacherOwnerOrAdminHandler
        : AuthorizationHandler<TeacherOwnerOrAdminRequirement, int>
    {
        private readonly SiSDBDbContext _context;

        public TeacherOwnerOrAdminHandler(
            SiSDBDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TeacherOwnerOrAdminRequirement requirement,
            int teacherId)
        {
            // Admin و SuperAdmin عندهم صلاحية كاملة
            if (context.User.IsInRole("Admin") ||
                context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return;
            }

            // الحصول على UserId من JWT
            var userIdClaim =
                context.User.FindFirstValue("id")
                ?? context.User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                userIdClaim,
                out int authenticatedUserId))
            {
                return;
            }

            // البحث عن الأستاذ باستخدام TeacherId
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t =>
                    t.TeacherId == teacherId);

            if (teacher == null)
            {
                return;
            }

            // المقارنة الصحيحة:
            // UserId الموجود بالـ JWT
            // مع UserId المرتبط بالأستاذ
            if (teacher.UserId == authenticatedUserId)
            {
                context.Succeed(requirement);
            }
        }
    }
}