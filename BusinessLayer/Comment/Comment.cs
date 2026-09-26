using BusinessLayer.Comment.CommentDto;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Comment
{
    public class Comment
    {
        private readonly SiSDBDbContext _context;

        public Comment(SiSDBDbContext context)
        {
            _context = context;
        }

        public List<GetCommentDto> GetAllComments()
        {
            return _context.Comments
                .Select(c => new GetCommentDto
                {
                    CommentId = c.CommentId,
                    LessonId = c.LessonId,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                })
                .ToList();
        }

        public GetCommentDto? GetCommentById(int commentId)
        {
            if (commentId <= 0) return null;

            return _context.Comments
                .Where(c => c.CommentId == commentId)
                .Select(c => new GetCommentDto
                {
                    CommentId = c.CommentId,
                    LessonId = c.LessonId,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefault();
        }

        public List<GetCommentDto> GetCommentsByLessonId(int lessonId)
        {
            if (lessonId <= 0) return new List<GetCommentDto>();

            return _context.Comments
                .Where(c => c.LessonId == lessonId)
                .Select(c => new GetCommentDto
                {
                    CommentId = c.CommentId,
                    LessonId = c.LessonId,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                })
                .ToList();
        }

        public List<GetCommentWithFullDetailsDto> GetCommentsWithFullDetailsByLessonId(int lessonId)
        {
            if (lessonId <= 0) return new List<GetCommentWithFullDetailsDto>();

            return _context.Comments
                .Where(c => c.LessonId == lessonId)
                .Join(_context.Lessons, com => com.LessonId, l => l.LessonId, (com, l) => new { com, l })
                .Join(_context.Users, temp => temp.com.UserId, u => u.UserId, (temp, u) => new GetCommentWithFullDetailsDto
                {
                    CommentId = temp.com.CommentId,
                    Content = temp.com.Content,
                    CreatedAt = temp.com.CreatedAt,
                    LessonId = temp.l.LessonId,
                    LessonTitle = temp.l.Title,
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    SecondName = u.SecondName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                })
                .ToList();
        }

        public List<GetCommentWithFullDetailsDto> GetAllCommentsWithFullDetails()
        {
            return _context.Comments
                .Join(_context.Lessons, com => com.LessonId, l => l.LessonId, (com, l) => new { com, l })
                .Join(_context.Users, temp => temp.com.UserId, u => u.UserId, (temp, u) => new GetCommentWithFullDetailsDto
                {
                    CommentId = temp.com.CommentId,
                    Content = temp.com.Content,
                    CreatedAt = temp.com.CreatedAt,
                    LessonId = temp.l.LessonId,
                    LessonTitle = temp.l.Title,
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    SecondName = u.SecondName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                })
                .ToList();
        }

        public int AddNewComment(AddCommentDto addDto)
        {
            if (addDto == null) return 0;

            bool lessonExists = _context.Lessons.Any(l => l.LessonId == addDto.LessonId);
            if (!lessonExists) return -1; 

            bool userExists = _context.Users.Any(u => u.UserId == addDto.UserId);
            if (!userExists) return -2; 

            var comment = new DataLayer.Models.Entities.Comment
            {
                LessonId = addDto.LessonId,
                UserId = addDto.UserId,
                Content = addDto.Content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            bool isSaved = _context.SaveChanges() > 0;

            return isSaved ? comment.CommentId : 0;
        }

        public bool UpdateComment(UpdateCommentDto updateDto)
        {
            if (updateDto == null || updateDto.CommentId <= 0) return false;

            var comment = _context.Comments.Find(updateDto.CommentId);
            if (comment == null) return false;

            if (!string.IsNullOrWhiteSpace(updateDto.Content))
            {
                comment.Content = updateDto.Content;
            }

            _context.Comments.Update(comment);
            return _context.SaveChanges() > 0;
        }


        public bool DeleteComment(int commentId)
        {
            if (commentId <= 0) return false;

            var comment = _context.Comments.Find(commentId);
            if (comment == null) return false;

            _context.Comments.Remove(comment);
            return _context.SaveChanges() > 0;
        }
    }
}