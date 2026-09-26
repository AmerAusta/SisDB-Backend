using Microsoft.AspNetCore.Authorization;

namespace ApiLayer.Authorization
{
    public class TeacherOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}