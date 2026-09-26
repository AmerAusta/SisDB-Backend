using BusinessLayer.Attendance.AttendanceDto;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Attendance
{
    public class Attendance
    {
        private readonly SiSDBDbContext _context;

        public Attendance(SiSDBDbContext context)
        {
            _context = context;
        }

        public GetAttendanceDto? GetAttendanceById(int attendanceId)
        {
            if (attendanceId <= 0) return null;

            var attendance = _context.Attendances.Find(attendanceId);
            if (attendance == null) return null;

            return new GetAttendanceDto
            {
                AttendanceId = attendance.AttendanceId,
                StudentId = attendance.StudentId,
                LessonId = attendance.LessonId,
                Date = attendance.Date,
                Status = attendance.Status
            };
        }

        public List<GetAttendanceDto> GetAttendancesByStudentId(int studentId)
        {
            if (studentId <= 0) return new List<GetAttendanceDto>();

            return _context.Attendances 
                .Where(a => a.StudentId == studentId)
                .Select(a => new GetAttendanceDto
                {
                    AttendanceId = a.AttendanceId,
                    StudentId = a.StudentId,
                    LessonId = a.LessonId,
                    Date = a.Date,
                    Status = a.Status
                })
                .ToList();
        }

        public List<GetAttendanceWithFullDetailsDto> GetAttendancesWithFullDetailsByStudentId(int studentId)
        {
            if (studentId <= 0) return new List<GetAttendanceWithFullDetailsDto>();

            return _context.Attendances
                .Where(a => a.StudentId == studentId) 
                .Join(_context.Students, a => a.StudentId, s => s.StudentId, (a, s) => new { a, s })
                .Join(_context.Users, temp => temp.s.UserId, u => u.UserId, (temp, u) => new { temp.a, temp.s, u })
                .Join(_context.Classes, temp => temp.s.ClassId, c => c.ClassId, (temp, c) => new { temp.a, temp.s, temp.u, c })
                .Join(_context.Lessons, temp => temp.a.LessonId, l => l.LessonId, (temp, l) => new GetAttendanceWithFullDetailsDto
                {
                    AttendanceId = temp.a.AttendanceId,
                    Date = temp.a.Date,
                    Status = temp.a.Status,

                    StudentId = temp.s.StudentId,
                    UserId = temp.u.UserId,
                    FirstName = temp.u.FirstName,
                    SecondName = temp.u.SecondName,
                    LastName = temp.u.LastName,
                    Email = temp.u.Email,
                    PhoneNumber = temp.u.PhoneNumber,
                    EnrollmentDate = temp.s.EnrollmentDate,

                    ClassId = temp.c.ClassId,
                    ClassName = temp.c.ClassName,
                    Branch = temp.c.Branch,

                    LessonId = l.LessonId,
                    Title = l.Title,
                    VideoUrl = l.VideoUrl,
                    PdfUrl = l.PdfUrl,
                    IsPublished = l.IsPublished
                })
                .ToList();
        }

        public GetAttendanceWithFullDetailsDto? GetAttendanceWithFullDetailsById(int attendanceId)
        {
            if (attendanceId <= 0) return null;

            return _context.Attendances
                .Where(a => a.AttendanceId == attendanceId)
                .Join(_context.Students, a => a.StudentId, s => s.StudentId, (a, s) => new { a, s })
                .Join(_context.Users, temp => temp.s.UserId, u => u.UserId, (temp, u) => new { temp.a, temp.s, u })
                .Join(_context.Classes, temp => temp.s.ClassId, c => c.ClassId, (temp, c) => new { temp.a, temp.s, temp.u, c })
                .Join(_context.Lessons, temp => temp.a.LessonId, l => l.LessonId, (temp, l) => new GetAttendanceWithFullDetailsDto
                {
                    AttendanceId = temp.a.AttendanceId,
                    Date = temp.a.Date,
                    Status = temp.a.Status,

                    StudentId = temp.s.StudentId,
                    UserId = temp.u.UserId,
                    FirstName = temp.u.FirstName,
                    SecondName = temp.u.SecondName,
                    LastName = temp.u.LastName,
                    Email = temp.u.Email,
                    PhoneNumber = temp.u.PhoneNumber,
                    EnrollmentDate = temp.s.EnrollmentDate,

                    ClassId = temp.c.ClassId,
                    ClassName = temp.c.ClassName,
                    Branch = temp.c.Branch,

                    LessonId = l.LessonId,
                    Title = l.Title,
                    VideoUrl = l.VideoUrl,
                    PdfUrl = l.PdfUrl,
                    IsPublished = l.IsPublished
                })
                .FirstOrDefault();
        }

        public List<GetAttendanceWithFullDetailsDto> GetAllAttendancesWithFullDetails()
        {
            return _context.Attendances
                .Join(_context.Students, a => a.StudentId, s => s.StudentId, (a, s) => new { a, s })
                .Join(_context.Users, temp => temp.s.UserId, u => u.UserId, (temp, u) => new { temp.a, temp.s, u })
                .Join(_context.Classes, temp => temp.s.ClassId, c => c.ClassId, (temp, c) => new { temp.a, temp.s, temp.u, c })
                .Join(_context.Lessons, temp => temp.a.LessonId, l => l.LessonId, (temp, l) => new GetAttendanceWithFullDetailsDto
                {
                    AttendanceId = temp.a.AttendanceId,
                    Date = temp.a.Date,
                    Status = temp.a.Status,

                    StudentId = temp.s.StudentId,
                    UserId = temp.u.UserId,
                    FirstName = temp.u.FirstName,
                    SecondName = temp.u.SecondName,
                    LastName = temp.u.LastName,
                    Email = temp.u.Email,
                    PhoneNumber = temp.u.PhoneNumber,
                    EnrollmentDate = temp.s.EnrollmentDate,

                    ClassId = temp.c.ClassId,
                    ClassName = temp.c.ClassName,
                    Branch = temp.c.Branch,

                    LessonId = l.LessonId,
                    Title = l.Title,
                    VideoUrl = l.VideoUrl,
                    PdfUrl = l.PdfUrl,
                    IsPublished = l.IsPublished
                })
                .ToList();
        }

        public List<GetAttendanceWithFullDetailsDto> GetAttendancesWithFullDetailsByLessonId(int lessonId)
        {
            if (lessonId <= 0) return new List<GetAttendanceWithFullDetailsDto>();

            return _context.Attendances
                .Where(a => a.LessonId == lessonId)
                .Join(_context.Students, a => a.StudentId, s => s.StudentId, (a, s) => new { a, s })
                .Join(_context.Users, temp => temp.s.UserId, u => u.UserId, (temp, u) => new { temp.a, temp.s, u })
                .Join(_context.Classes, temp => temp.s.ClassId, c => c.ClassId, (temp, c) => new { temp.a, temp.s, temp.u, c })
                .Join(_context.Lessons, temp => temp.a.LessonId, l => l.LessonId, (temp, l) => new GetAttendanceWithFullDetailsDto
                {
                    AttendanceId = temp.a.AttendanceId,
                    Date = temp.a.Date,
                    Status = temp.a.Status,

                    StudentId = temp.s.StudentId,
                    UserId = temp.u.UserId,
                    FirstName = temp.u.FirstName,
                    SecondName = temp.u.SecondName,
                    LastName = temp.u.LastName,
                    Email = temp.u.Email,
                    PhoneNumber = temp.u.PhoneNumber,
                    EnrollmentDate = temp.s.EnrollmentDate,

                    ClassId = temp.c.ClassId,
                    ClassName = temp.c.ClassName,
                    Branch = temp.c.Branch,

                    LessonId = l.LessonId,
                    Title = l.Title,
                    VideoUrl = l.VideoUrl,
                    PdfUrl = l.PdfUrl,
                    IsPublished = l.IsPublished
                })
                .ToList();
        }

        public List<GetAttendanceDto> GetAllAttendances()
        {
            return _context.Attendances 
                .Select(a => new GetAttendanceDto
                {
                    AttendanceId = a.AttendanceId,
                    StudentId = a.StudentId,
                    LessonId = a.LessonId,
                    Date = a.Date,
                    Status = a.Status
                })
                .ToList();
        }

        public int AddNewAttendance(AddAttendanceDto addDto)
        {
            if (addDto == null) return 0;

           
            var student = _context.Students.Find(addDto.StudentId);
            if (student == null) return -1;

            var lesson = _context.Lessons.Find(addDto.LessonId);
            if (lesson == null) return -2;

            var targetDate =  DateOnly.FromDateTime(DateTime.Now) ;


            var existingAttendance = _context.Attendances
                .FirstOrDefault(a => a.StudentId == addDto.StudentId && a.LessonId == addDto.LessonId);

            if (existingAttendance != null)
            {
                existingAttendance.Date = targetDate;


                _context.SaveChanges();
                return existingAttendance.AttendanceId; 
            }

            var attendance = new DataLayer.Models.Entities.Attendance
            {
                StudentId = addDto.StudentId,
                LessonId = addDto.LessonId,
                Date = targetDate,
                Status="تم الحضور"
            };

            _context.Attendances.Add(attendance);
            int rowsAffected = _context.SaveChanges();

            return rowsAffected > 0 ? attendance.AttendanceId : 0;
        }

        public bool UpdateAttendance(UpdateAttendanceDto updateDto)
        {
            if (updateDto == null || updateDto.AttendanceId <= 0) return false;

            var attendance = _context.Attendances.Find(updateDto.AttendanceId); 
            if (attendance == null) return false;

            if (updateDto.StudentId.HasValue && updateDto.StudentId.Value > 0)
            {
                var student = _context.Students.Find(updateDto.StudentId.Value);
                if (student == null) return false;
                attendance.StudentId = updateDto.StudentId.Value;
            }

            if (updateDto.LessonId.HasValue && updateDto.LessonId.Value > 0)
            {
                var lesson = _context.Lessons.Find(updateDto.LessonId.Value);
                if (lesson == null) return false;
                attendance.LessonId = updateDto.LessonId.Value;
            }

            if (updateDto.Date.HasValue)
            {
                attendance.Date = updateDto.Date.Value;
            }


            return _context.SaveChanges() > 0;
        }

        public bool DeleteAttendance(int attendanceId)
        {
            if (attendanceId <= 0) return false;

            var attendance = _context.Attendances.Find(attendanceId);
            if (attendance == null) return false;

            _context.Attendances.Remove(attendance);
            return _context.SaveChanges() > 0;
        }
    }
}