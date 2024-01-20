using AcademiaSys.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaSys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly dBContext _dBContext;

        private const string ValidAccessToken = "access1234";

        public SubjectsController()
        {
            _dBContext = new dBContext();
        }
        [HttpGet("getMarks")]
        public IActionResult GetMarks()
        {
            try
            {
                var allMarks = _dBContext.Subjects.ToList();

                return Ok(allMarks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("updateMarks")]
        public IActionResult UpdateMarke([FromBody] UpdateMarksRequest updateMarksRequest)
        {
            try
            {
                var result = _dBContext.Subjects.Find(updateMarksRequest.Id);

                if (result == null)
                {
                    return NotFound("Data not found");
                }

                result.Mat = updateMarksRequest.Mat;
                result.Eng = updateMarksRequest.Eng;
                result.Sci = updateMarksRequest.Sci;
                result.His = updateMarksRequest.His;
                result.Geo = updateMarksRequest.Geo;
                result.Lab = updateMarksRequest.Lab;

                // Save changes to the database
                _dBContext.SaveChanges();

                var response = new Response
                {
                    Success = true,
                    Message = "Student marks updated successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("highestMarksBySubject")]
        public IActionResult GetHighestMarksBySubject()
        {
            try
            {
                var subjectsData = _dBContext.Subjects.ToList();

                if (subjectsData.Count == 0)
                {
                    return Ok("No data in the Subjects table.");
                }

                var highestMarksBySubject = new List<object>();

                // Get property names excluding SID
                var subjectProperties = typeof(Subjects).GetProperties()
                    .Where(p => p.PropertyType == typeof(int?) && p.Name != "SID")
                    .Select(p => p.Name);

                // Iterate over each subject column
                foreach (var propertyName in subjectProperties)
                {
                    // Find the highest value for the current subject
                    var highestMarks = subjectsData.Max(subject => subject.GetType().GetProperty(propertyName)?.GetValue(subject) as int?);

                    // Find the student with the highest marks in the current subject
                    var studentWithHighestMarks = subjectsData
                        .FirstOrDefault(subject => subject.GetType().GetProperty(propertyName)?.GetValue(subject) as int? == highestMarks);

                    // Retrieve the student's name from the Students table using the SID
                    // Retrieve the student's name from the Students table using the SID
                    var studentName = string.Empty;
                    if (studentWithHighestMarks?.SID != null)
                    {
                        studentName = _dBContext.Students
                            .Where(student => student.Id == studentWithHighestMarks.SID)
                            .Select(student => student.Fullname)
                            .FirstOrDefault();
                    }


                    // Add the result to the list
                    highestMarksBySubject.Add(new { Subject = propertyName, HighestMarks = highestMarks, HighestScorer = studentName });
                }

                return Ok(highestMarksBySubject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpPost("updateRandomMarks")]
        public IActionResult UpdateRandomMarks([FromQuery] string accessToken)
        {
            try
            {
                // Check if the provided accessToken is valid
                if (accessToken != ValidAccessToken)
                {
                    return Unauthorized("Invalid access token");
                }

                var subjects = _dBContext.Subjects.ToList();

                var random = new Random();

                foreach (var subject in subjects)
                {
                    // Check if the column value is 0, then update with random marks
                    if (subject.Mat == 0)
                        subject.Mat = random.Next(15, 101);

                    if (subject.Eng == 0)
                        subject.Eng = random.Next(15, 101);

                    if (subject.Sci == 0)
                        subject.Sci = random.Next(15, 101);

                    if (subject.His == 0)
                        subject.His = random.Next(15, 101);

                    if (subject.Geo == 0)
                        subject.Geo = random.Next(15, 101);

                    if (subject.Lab == 0)
                        subject.Lab = random.Next(15, 101);
                }

                _dBContext.SaveChanges();

                var response = new Response
                {
                    Success = true,
                    Message = "Random marks updated successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


    }
}
