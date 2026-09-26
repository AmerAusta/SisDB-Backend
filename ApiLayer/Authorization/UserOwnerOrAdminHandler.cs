using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DataLayer.Data;

namespace ApiLayer.Authorization
{
    public class UserOwnerOrAdminHandler : AuthorizationHandler<UserOwnerOrAdminRequirement, int>
    {
        private readonly SiSDBDbContext _context;

        public UserOwnerOrAdminHandler(SiSDBDbContext context)
        {
            _context = context;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserOwnerOrAdminRequirement requirement,
            int studentId)
        {
            // 1. التجاوز التلقائي للـ SuperAdmin والـ Admin
            if (context.User.IsInRole("SuperAdmin") || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // 2. قراءة UserId القادم من التوكن بصيغة "id"
            var userIdClaim = context.User.FindFirstValue("id")
                           ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? context.User.FindFirstValue("UserId");

            if (!int.TryParse(userIdClaim, out int authenticatedUserId))
            {
                return Task.CompletedTask;
            }

            // 3. جلب سجل الطالب من قاعدة البيانات
            var student = _context.Students.Find(studentId);

            if (student == null)
            {
                return Task.CompletedTask;
            }

            // 4. التحقق من صلاحية المعلم (Teacher)
            if (context.User.IsInRole("Teacher"))
            {
                bool teachesStudent = _context.Assignments.Any(a =>
                    a.TeacherId == authenticatedUserId &&
                    a.ClassId == student.ClassId);

                if (teachesStudent)
                {
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }

            // 5. التحقق من الملكية للطالب أو ولي الأمر
            bool isStudentOwner = student.UserId == authenticatedUserId;
            bool isParentOwner = student.ParentId.HasValue && student.ParentId.Value == authenticatedUserId;

            if (isStudentOwner || isParentOwner)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}