using Microsoft.AspNetCore.Mvc;
using AcademiaSys.Models;
using AcademiaSys.Services;
using Microsoft.EntityFrameworkCore;
using Azure.Core;

namespace AcademiaSys.Controllers
{
    public class StudentsController :    ControllerBase

    {
        private readonly dBContext _dBContext;

        private const string ValidAccessToken = "access1234";

        public StudentsController()
        {
            _dBContext = new dBContext();
        }

        [HttpPost("admit")]
        public IActionResult Admission([FromBody] Students student)
        {
            try
            {
                // Check if the user already exists
                if (_dBContext.Students.Any(u => u.Email == student.Email || u.Fullname == student.Fullname))
                {
                    var res = new Response
                    {
                        Success = false,
                        Message = "User already exists"
                    };

                    return Ok(res);
                }


                _dBContext.Students.Add(student);
                _dBContext.SaveChanges();

                var response = new Response
                {
                    Success = true,
                    Message = "Student admitted successfully"
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

        [HttpGet("getStd")]
        public IActionResult GetStd()
        {
            try
            {
                // Define the three roles
                var predefinedRoles = new List<object>
            {
                new { Name = "I", Value = "I" },
                new { Name = "II", Value = "II" },
                new { Name = "III", Value = "III" },
                new { Name = "IV", Value = "IV" },
                new { Name = "V", Value = "V" },
                new { Name = "VI", Value = "VI" },
                new { Name = "VII", Value = "VII" },
                new { Name = "VIII", Value = "VIII" },
                new { Name = "IX", Value = "IX" },
                new { Name = "X", Value = "X" }
            };

                return Ok(predefinedRoles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpDelete("deleteStudent")]
        public IActionResult DeleteUser([FromBody] DeleteUserRequest request)
        {
            try
            {
                // Check if the provided accessToken is valid
                if (request.Key != ValidAccessToken)
                {
                    return Unauthorized("Invalid access token");
                }

                // Find the user by ID
                var user = _dBContext.Students.Find(request.Id);

                // Check if the user exists
                if (user == null)
                {
                    return NotFound("Student not found");
                }

                // Remove the user
                _dBContext.Students.Remove(user);
                _dBContext.SaveChanges();

                var response = new Response
                {
                    Success = true,
                    Message = "Student Record deleted successfully"
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

        [HttpGet("allStudents")]
        public IActionResult GetAllStudents()
        {
            try
            {
                var allUsers = _dBContext.Students.ToList();

                return Ok(allUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("updateStudent")]
        public IActionResult UpdateUser([FromBody] Students updateStudentRequest)
        {
            try
            {
                // Check if the Student exists
                var existingStudent = _dBContext.Students.Find(updateStudentRequest.Id);

                if (existingStudent == null)
                {
                    return NotFound("Student not found");
                }

                existingStudent.Fullname = updateStudentRequest.Fullname;
                existingStudent.Contact = updateStudentRequest.Contact;
                existingStudent.Email = updateStudentRequest.Email;
                existingStudent.Std = updateStudentRequest.Std;
                existingStudent.DOB = updateStudentRequest.DOB;
                existingStudent.Gender = updateStudentRequest.Gender;

                _dBContext.SaveChanges();

                var response = new Response
                {
                    Success = true,
                    Message = "Student data updated successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("totalStudentsByStd")]
        public IActionResult GetTotalStudentsByStd()
        {
            try
            {
                var allStandards = new List<string> { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };

                var totalStudentsByStd = allStandards
                    .Select(std => new
                    {
                        Standard = std,
                        TotalStudents = _dBContext.Students.Count(s => s.Std == std)
                    })
                    .ToList();

                return Ok(totalStudentsByStd);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("admitBulk")]
        public IActionResult AdmissionBulk([FromQuery] string accessToken, [FromBody] List<Students> students)
        {
            try
            {
                // Check if the provided accessToken is valid
                if (accessToken != ValidAccessToken)
                {
                    return Unauthorized("Invalid access token");
                }

                if (students == null || students.Count == 0)
                {
                    return BadRequest("No students provided for admission.");
                }

                // Check if any of the students already exist
                var existingStudents = _dBContext.Students
                    .AsEnumerable()  // Switch to client-side evaluation
                    .Where(u => students.Any(s => s.Email == u.Email || s.Fullname == u.Fullname))
                    .ToList();

                if (existingStudents.Any())
                {
                    var res = new Response
                    {
                        Success = false,
                        Message = "Some students already exist",
                        Data = existingStudents
                    };

                    return Ok(res);
                }

                // Add all students in bulk
                _dBContext.Students.AddRange(students);
                _dBContext.SaveChanges();

                var response = new Response
                {
                    Success = true,
                    Message = "Students admitted successfully"
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


    }

}
