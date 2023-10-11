using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly DemoDB service;

        public PersonController(DemoDB service)
        {
            this.service = service;
        }
        [HttpGet]
        [Route("GetPerson")]
        [Produces("application/json")]
        public IActionResult GetPerson(int id)
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
        [Route("GetAllStudents")]
        [Produces("application/json")]
        public IActionResult GetAllStudents()
        {
        
            List<PersonDto> results;

            try
            {
                results = service.GetCurrentPeopleOfType(PersonType.Student);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server error contact EIS: GetLatestTest");
            }

            return Ok(results);
        }
        [HttpGet]
        [Route("GetAllTeachers")]
        [Produces("application/json")]
        public IActionResult GetAllTeachers()
        {

            List<PersonDto> results;

            try
            {
                results = service.GetCurrentPeopleOfType(PersonType.Teacher);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server error contact EIS: GetLatestTest");
            }

            return Ok(results);
        }
        [HttpPost]
        [Route("CreatePerson")]
        [Produces("application/json")]
        public IActionResult CreatePerson(PersonDto person)
        {

            Person result;

            try
            {
                result = service.CreatePerson(person);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex.Message);
                return BadRequest("Invalid Data");
            }

            return Ok(result);
        }
        [HttpPut]
        [Route("UpdatePerson")]
        //[Produces("application/json")]
        public IActionResult UpdatePerson(int id,Person update)
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
