using BusinessLayer.Classes.ClassDto;
using BusinessLayer.User;
using DataLayer.Data;
using DataLayer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Classes
{
    public class Class
    {
        private readonly SiSDBDbContext _context;

        public Class(SiSDBDbContext context)
        {
            _context = context;
        }

        public GetClassDto? GetClassById(int classId)
        {
            if (classId <= 0) return null;

            var cls = _context.Classes.Find(classId);

            if (cls == null) return null;

            return new GetClassDto
            {
                ClassId = cls.ClassId,
                ClassName = cls.ClassName,
                Branch = cls.Branch,
                AcademicYear = cls.AcademicYear,
                Capacity = cls.Capacity,
                CurrentCapacity = cls.CurrentCapacity,
            };
        }

        public List<GetClassDto> GetAllClasses()
        {
            return _context.Classes.Select(cls => new GetClassDto
            {
                ClassId = cls.ClassId,
                ClassName = cls.ClassName,
                Branch = cls.Branch,
                AcademicYear = cls.AcademicYear,
                Capacity = cls.Capacity,
                CurrentCapacity = cls.CurrentCapacity,

            }).ToList();
        }

        public GetClassDto? GetClassByName(string className, string? academicYear = null)
        {
            if (string.IsNullOrWhiteSpace(className)) return null;

            string searchName = className.Trim();

            var cls = _context.Classes.FirstOrDefault(c =>
                c.ClassName.Contains(searchName) &&
                (string.IsNullOrWhiteSpace(academicYear) || c.AcademicYear == academicYear.Trim())
            );

            if (cls == null) return null;

            return new GetClassDto
            {
                ClassId = cls.ClassId,
                ClassName = cls.ClassName,
                Branch = cls.Branch,
                AcademicYear = cls.AcademicYear,
                Capacity = cls.Capacity,
                CurrentCapacity = cls.CurrentCapacity,
            };
        }

        public GetClassDto? GetClassByBranch(string classBranch, string? academicYear = null)
        {
            if (string.IsNullOrWhiteSpace(classBranch)) return null;

            string searchName = classBranch.Trim();

            var cls = _context.Classes.FirstOrDefault(c =>
                c.Branch.Contains(searchName) &&
                (string.IsNullOrWhiteSpace(academicYear) || c.AcademicYear == academicYear.Trim())
            );

            if (cls == null) return null;

            return new GetClassDto
            {
                ClassId = cls.ClassId,
                ClassName = cls.ClassName,
                Branch = cls.Branch,
                AcademicYear = cls.AcademicYear,
                Capacity = cls.Capacity,
                CurrentCapacity= cls.CurrentCapacity,
            };
        }

        public bool IsClassNameExists(string ClassName, int excludeClassId = 0)
        {
            return _context.Classes.Any(s => s.ClassName.ToLower() == ClassName.ToLower() && s.ClassId != excludeClassId);
        }

        public bool AddNewClass(AddClassDto addClass)
        {
            if (addClass == null) return false;

            try
            {
                var classEntity = new DataLayer.Models.Entities.Class
                {
                    ClassName = addClass.ClassName,
                    Branch = addClass.Branch,
                    AcademicYear = addClass.AcademicYear,
                    Capacity = addClass.Capacity,
                    CurrentCapacity=0
                };

                _context.Classes.Add(classEntity);
                int rowsAffected = _context.SaveChanges();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateClass(UpdateClassDto updateClass)
        {
            if (updateClass == null || updateClass.ClassId <= 0) return false;

            try
            {
                var cls = _context.Classes.Find(updateClass.ClassId);

                if (cls == null) return false;
                
                cls.ClassName = !string.IsNullOrWhiteSpace(updateClass.ClassName) ? updateClass.ClassName : cls.ClassName;
                
                cls.Branch = !string.IsNullOrWhiteSpace(updateClass.Branch) ? updateClass.Branch : cls.Branch;
                
                cls.AcademicYear = !string.IsNullOrWhiteSpace(updateClass.AcademicYear) ? updateClass.AcademicYear : cls.AcademicYear;

                if (updateClass.Capacity > 0&&updateClass.Capacity>=cls.CurrentCapacity)
                    cls.Capacity = updateClass.Capacity??cls.Capacity;

                int rowsAffected = _context.SaveChanges();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteClass(int classId)
        {
            if (classId <= 0) return false;

            try
            {
                var cls = _context.Classes.Find(classId);

                if (cls == null) return false;

                _context.Classes.Remove(cls);
                int rowsAffected = _context.SaveChanges();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheackCapacity(int classId)
        {
            var cls = _context.Classes.Find(classId);

            if (cls != null &&cls.CurrentCapacity > 0) return false;
            
            return true;
        }
    }
}
