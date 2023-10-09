using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly DemoDB service;

        public StudentController(DemoDB service)
        {
            this.service = service;
        }
        [HttpGet]
        [Route("GetStudent")]
        [Produces("application/json")]
        public IActionResult GetStudent(int id)
        {

            PersonDto result;

            try
            {
                result = service.Get(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server error contact EIS: GetLatestTest");
            }

            return Ok(result);
        }
        [HttpGet]
        [Route("GetStudentCourse")]
        [Produces("application/json")]
        public IActionResult GetStudentCourse(int id)
        {

            List<CourseDto> result;

            try
            {

                result = service.GetStudentCourse(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server error contact EIS: GetLatestTest");
            }

            return Ok(result);
        }
        [HttpPut]
        [Route("AddCourse")]
        [Produces("application/json")]
        public IActionResult AddCourse(int stuId, int courseId)
        {

            bool result;

            try
            {
                result = service.AddStudentToCourse(stuId,courseId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server error contact EIS: GetLatestTest");
            }

            return Ok(result);
        }
        [HttpGet]
        [Route("GetAllStudents")]
        [Produces("application/json")]
        public IActionResult GetAllStudents()
        {
        
            List<Person> results;

            try
            {
                results = service.GetCurrentStudents();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server error contact EIS: GetLatestTest");
            }

            return Ok(results);
        }

        [HttpPost]
        [Route("CreateStudent")]
        //[Produces("application/json")]
        public String CreateStudent(Person student)
        {

            Person result;

            try
            {
                result = service.CreateStudent(student);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex.Message);
                return "Invalid Data";
            }

            return "OK";
        }
        [HttpPut]
        [Route("UpdateStudent")]
        //[Produces("application/json")]
        public IActionResult UpdateStudent(int id,Person update)
        {

            if (!service.Update(id,update))
            {
                return BadRequest("Invalid request");
            }

            return Ok(200);
        }

        [HttpDelete]
        [Route("Delete")]
        [Produces("application/json")]
        public String DeleteStudent(int id)
        {
            try
            {
                service.DeleteStudent(id);
                
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return "Invalid Data";
            }

            return "Ok";
        }

    }
}
