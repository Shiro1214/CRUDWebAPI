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
        public IActionResult CreateCourse(CourseDto course)
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

            List<CourseDto> result;

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

        [HttpGet]
        [Route("GetCourse")]
        [Produces("application/json")]
        public IActionResult GetCourses(int id)
        {

            CourseDto result;

            try
            {
                result = service.GetCourse(id);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return BadRequest("Invalid Data");
            }

            return Ok(result);
        }
        [HttpDelete]
        [Route("DeleteCourse")]
        [Produces("application/json")]
        public String DeleteCourse(int id)
        {
            try
            {
                service.DeleteCourse(id);

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
