using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly DemoDB service;

        public CourseController(DemoDB service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("CreateCourse")]
        [Produces("application/json")]
        public IActionResult CreateCourse(Course course)
        {

            Course result;

            try
            {
                result = service.CreateCourse(course);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return BadRequest("Invalid Data");
            }

            return Ok(result);
        }


        [HttpGet]
        [Route("GetCourses")]
        [Produces("application/json")]
        public IActionResult GetCourses()
        {

            List<Course> result;

            try
            {
                result = service.GetCourses();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return BadRequest("Invalid Data");
            }

            return Ok(result);
        }
    }
}
