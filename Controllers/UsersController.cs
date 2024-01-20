using AcademiaSys.Models;
using AcademiaSys.Services;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademiaSys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly dBContext _dBContext;

        private const string ValidAccessToken = "access1234";

        public UsersController()
        {
            _dBContext = new dBContext();
        }

        [HttpPost("registration")]
        public IActionResult Registration(Users user)
        {
            try
            {
                // Check if the user already exists
                if (_dBContext.Users.Any(u => u.Email == user.Email))
                {
                    var res = new Response
                    {
                        Success = false,
                        Message = "User already exists"
                    };

                    return Ok(res);
                }

                user.Password = Services.PasswordHasher.HashPassword(user.Password);

                _dBContext.Users.Add(user);
                _dBContext.SaveChanges();

                var response = new Response
                {
                    Success = true,
                    Message = "User account created successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("deleteUser")]
        public IActionResult DeleteUser(DeleteUserRequest request)
        {
            try
            {
                // Check if the provided accessToken is valid
                if (request.Key != ValidAccessToken)
                {
                    return Unauthorized("Invalid access token");
                }

                // Find the user by ID
                var user = _dBContext.Users.Find(request.Id);

                // Check if the user exists
                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Remove the user
                _dBContext.Users.Remove(user);
                _dBContext.SaveChanges();

                var response = new Response
                {
                    Success = true,
                    Message = "User deleted successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("getRoles")]
        public IActionResult GetRoles()
        {
            try
            {
                // Define the three roles
                var predefinedRoles = new List<object>
            {
                new { name = "Principal", value = "Principal" },
                new { name = "Class Teacher", value = "Class Teacher" },
                new { name = "Teacher", value = "Teacher" }
            };

                return Ok(predefinedRoles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("allUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var allUsers = _dBContext.Users.ToList();

                return Ok(allUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("updateUser")]
        public IActionResult UpdateUser([FromBody] UpdateUserRequest updateUserRequest)
        {
            try
            {
                // Check if the user exists
                var existingUser = _dBContext.Users.Find(updateUserRequest.Id);

                if (existingUser == null)
                {
                    return NotFound("User not found");
                }

                // Update the user properties excluding the password
                existingUser.Fname = updateUserRequest.Fname;
                existingUser.Lname = updateUserRequest.Lname;
                existingUser.Email = updateUserRequest.Email;
                existingUser.Role = updateUserRequest.Role;

                // Save changes to the database
                _dBContext.SaveChanges();

                var response = new Response
                {
                    Success = true,
                    Message = "User updated successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("adduser")]
        public IActionResult AddUser([FromBody] AddUserRequest addUserRequest)
        {
            try
            {
                var newPassword = PasswordGenerator.GeneratePassword();

                // Send the Password to the user's email
                EmailService emailService = new EmailService();

                Response reply = emailService.SendOtpEmail(addUserRequest.Email, newPassword);

                // Create a new user
                var newUser = new Users
                {
                    Fname = addUserRequest.Fname,
                    Lname = addUserRequest.Lname,
                    Email = addUserRequest.Email,
                    Role = addUserRequest.Role,
                    Password = Services.PasswordHasher.HashPassword(newPassword)
                };

                // Add the new user to the database
                _dBContext.Users.Add(newUser);
                _dBContext.SaveChanges();

                // Return a structured response
                var response = new Response
                {
                    Success = true,
                    Message = "New Admin added successfully",
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return a structured error response
                var errorResponse = new Response
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpGet("dashboardStats")]
        public IActionResult GetDashboardStats()
        {
            try
            {
                // Total number of users
                int totalUsers = _dBContext.Users.Count();

                // Total number of students
                int totalStudents = _dBContext.Students.Count();

                // Number of male and female students
                int maleStudents = _dBContext.Students.Count(s => s.Gender == 'M');
                int femaleStudents = _dBContext.Students.Count(s => s.Gender == 'F');

                // Student with the highest total marks
                var studentWithHighestMarks = _dBContext.Subjects
                    .OrderByDescending(s => s.Total)
                    .FirstOrDefault();

                // Check if studentWithHighestMarks is null
                if (studentWithHighestMarks == null)
                {
                    var dashboardStatsWithoutHighestScorer = new
                    {
                        TotalUsers = totalUsers,
                        TotalStudents = totalStudents,
                        MaleStudents = maleStudents,
                        FemaleStudents = femaleStudents,
                        HighestScorerName = "N/A",
                        HighestScorerTotalMarks = 0
                    };

                    return Ok(dashboardStatsWithoutHighestScorer);
                }

                // Retrieve the student with the highest marks from the Students table
                var highestScorerName = _dBContext.Students
                    .FirstOrDefault(s => s.Id == studentWithHighestMarks.SID);

                var dashboardStats = new
                {
                    TotalUsers = totalUsers,
                    TotalStudents = totalStudents,
                    MaleStudents = maleStudents,
                    FemaleStudents = femaleStudents,
                    HighestScorerName = highestScorerName?.Fullname ?? "N/A",
                    HighestScorerTotalMarks = studentWithHighestMarks.Total
                };

                return Ok(dashboardStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }




    }
}
