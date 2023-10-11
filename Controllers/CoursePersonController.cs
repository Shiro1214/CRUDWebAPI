using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursePersonController : ControllerBase
    {
        private readonly DemoDB service;
        
        public CoursePersonController(DemoDB service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("/GetStudentGrade")]
        [Produces("application/json")]
        public IActionResult GetStudentGrade(int sid, int cid)
        {
            LetterGrade? res;
            try
            {
                res = service.GetStudentGrade(sid, cid);
            } catch (Exception ex)
            {
                return BadRequest("Invalid ID(s)");
            }
            return Ok(res.ToString());
        }
        [HttpPost]
        [Route("/ChangeStudentGrade")]
        [Produces("application/json")]
        public IActionResult ChangeStudentGrade(int sid, int cid, char grade)
        {
            bool res;
            int gradeIdx = Char.IsUpper(grade) ? grade - 'A' : Char.IsLower(grade) ? grade - 'a' : -1;

            if (gradeIdx < 0 || gradeIdx > 5 ) {
                return BadRequest("Invalid Grade");
            }
            try
            {
                res = service.ChangeStudentGrade(sid, cid, (LetterGrade)gradeIdx);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid ID(s)");
            }
            return Ok("Success: "+ res);
        }
    }
}
