using BusinessLayer.Classes.ClassDto;
using BusinessLayer.Role.RoleDto;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Role
{
    public class Role
    {
        private readonly SiSDBDbContext _context;

        public Role(SiSDBDbContext context)
        {
            _context = context;
        }

        public GetRoleDto? GetRoleById(int RoleId)
        {
            if (RoleId <= 0) return null;

            var Role = _context.Roles.Find(RoleId);

            if (Role == null) return null;

            return new GetRoleDto
            {
                RoleId = RoleId,
                RoleName=Role.RoleName,
                IsActive=Role.IsActive,
            };
        }

        public bool AddNewRole(AddRoleDto addRole)
        {
            if (addRole == null || string.IsNullOrWhiteSpace(addRole.RoleName)) return false;

            try
            {
                var roleEntity = new DataLayer.Models.Entities.Role
                {
                    RoleName = addRole.RoleName
                };

                _context.Roles.Add(roleEntity);
                int rowsAffected = _context.SaveChanges();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateRole(UpdateRoleDto updateRole)
        {
            if (updateRole == null || updateRole.RoleId <= 0) return false;

            try
            {
                var role = _context.Roles.Find(updateRole.RoleId);

                if (role == null) return false;

                role.RoleName = !string.IsNullOrWhiteSpace(updateRole.RoleName) ? updateRole.RoleName : role.RoleName;

                role.IsActive = updateRole.IsActive ?? role.IsActive;

                int rowsAffected = _context.SaveChanges();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteRole(int roleId)
        {
            if (roleId <= 0) return false;

            try
            {
                var role = _context.Roles.Find(roleId);

                if (role == null || !role.IsActive) return false;

                role.IsActive = false;

                int rowsAffected = _context.SaveChanges();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<GetRoleDto> GetAllRoles()
        {
            return _context.Roles.Select(role => new GetRoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                IsActive = role.IsActive,
            }).ToList();
        }

        public List<GetRoleDto> GetAllRolesIsActive()
        {
            return _context.Roles
                .Where(role => role.IsActive) 
                .Select(role => new GetRoleDto 
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName
                })
                .ToList();
        }

        public List<GetRoleDto> GetAllRolesIsActiveForAdd()
        {
            
            var excludedRoleIds = new[] { 1, 3, 4 };

            return _context.Roles
                .Where(role => role.IsActive && !excludedRoleIds.Contains(role.RoleId)) 
                .Select(role => new GetRoleDto
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName
                })
                .ToList();
        }

        public bool IsValidRoleId(int roleId)
        {
            return _context.Roles.Any(r => r.RoleId == roleId);
        }

        public bool IsStudentRole(int roleId)
        {
            if (roleId <= 0) return false;

            return _context.Roles.Any(r => r.RoleId == roleId && r.RoleName.ToLower() == "student");
        }

        public bool IsTeacherRole(int roleId)
        {
            if (roleId <= 0) return false;

            return _context.Roles.Any(r => r.RoleId == roleId && r.RoleName.ToLower() == "teacher");
        }
    }
}
