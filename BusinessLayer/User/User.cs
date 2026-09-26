using BusinessLayer.User.UserDto;
using DataLayer.Data;
using DataLayer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.User
{
    public class User
    {
        private readonly SiSDBDbContext _context;

        public User(SiSDBDbContext context)
        {
            _context = context;
        }

        public GetUserDto? GetUserById (int UserId)
        {
            if (UserId == 0) return null;

            var user = _context.Users.Find(UserId);

            if (user == null)
                return null;
            else
                return new GetUserDto
                { 
                    UserId=user.UserId,
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = user.RoleId,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                };

        }

        public List<GetUserDto> GetUsersByName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return new List<GetUserDto>();

            string searchName = fullName.Trim();

            return _context.Users
                .Where(u => (u.FirstName + " " + (u.SecondName != null ? u.SecondName + " " : "") + u.LastName)
                    .Contains(searchName))
                .Select(u => new GetUserDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    SecondName = u.SecondName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    RoleId = u.RoleId,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                })
                .ToList();
        }

        public int AddNewUser(AddUserDto AddUser)
        {
            if (AddUser == null) return 0;

            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(AddUser.Password);

            try
            {
                
                var userEntity = new DataLayer.Models.Entities.User
                {
                    FirstName = AddUser.FirstName,
                    SecondName = AddUser.SecondName,
                    LastName = AddUser.LastName,
                    Email = AddUser.Email,
                    PasswordHash = PasswordHash,
                    PhoneNumber = AddUser.PhoneNumber,
                    RoleId = AddUser.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                };

                _context.Users.Add(userEntity);

                int rowsAffected = _context.SaveChanges();

                
                if (rowsAffected > 0)
                {
                    return userEntity.UserId; 
                }

                return 0;
            }
            catch (Exception ex)
            {
                return 0; 
            }
        }

        public bool UpdateUser(UpdateUserDto updateUser)
        {
            if (updateUser == null) return false;

            try
            {
                var user = _context.Users.Find(updateUser.UserID);

                if (user == null) return false;

                user.FirstName = !string.IsNullOrWhiteSpace(updateUser.FirstName) ? updateUser.FirstName : user.FirstName;
                
                user.SecondName = !string.IsNullOrWhiteSpace(updateUser.SecondName) ? updateUser.SecondName : user.SecondName;
                
                user.LastName = !string.IsNullOrWhiteSpace(updateUser.LastName) ? updateUser.LastName : user.LastName;
                
                user.Email = !string.IsNullOrWhiteSpace(updateUser.Email) ? updateUser.Email : user.Email;
                
                user.PhoneNumber = !string.IsNullOrWhiteSpace(updateUser.PhoneNumber) ? updateUser.PhoneNumber : user.PhoneNumber;

                user.IsActive = updateUser.IsActive ?? user.IsActive;

     
                

                if (!string.IsNullOrWhiteSpace(updateUser.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUser.Password);
                }

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateStudentorTeacher(UpdateUserDto updateUser)
        {
            if (updateUser == null) return false;

            try
            {
                var user = _context.Users.Find(updateUser.UserID);

                if (user == null) return false;

                user.FirstName = !string.IsNullOrWhiteSpace(updateUser.FirstName) ? updateUser.FirstName : user.FirstName;
                user.SecondName = !string.IsNullOrWhiteSpace(updateUser.SecondName) ? updateUser.SecondName : user.SecondName;
                user.LastName = !string.IsNullOrWhiteSpace(updateUser.LastName) ? updateUser.LastName : user.LastName;
                user.Email = !string.IsNullOrWhiteSpace(updateUser.Email) ? updateUser.Email : user.Email;
                user.PhoneNumber = !string.IsNullOrWhiteSpace(updateUser.PhoneNumber) ? updateUser.PhoneNumber : user.PhoneNumber;
                user.IsActive = updateUser.IsActive ?? user.IsActive;

                if (!string.IsNullOrWhiteSpace(updateUser.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUser.Password);
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteUser(int UserId)
        {
            if(UserId <=2) return false;


            try
            {
                var user = _context.Users.Find(UserId);

                if (user == null) return false;

                user.IsActive = false;

                int rowsAffected = _context.SaveChanges();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        
        }

        public bool IsEmailExists(string email, int currentUserId=0)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string searchEmail = email.Trim().ToLower();

            return _context.Users.Any(u => u.Email.ToLower() == searchEmail && u.UserId != currentUserId);
        }

        public bool IsPhoneNumberExists(string phoneNumber, int currentUserId=0)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            string searchPhone = phoneNumber.Trim();

            return _context.Users.Any(u => u.PhoneNumber == searchPhone && u.UserId != currentUserId);
        }

       
    }
}
