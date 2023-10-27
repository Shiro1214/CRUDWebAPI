using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class CourseDto
    {
        public virtual int ID { get; set; }
        public virtual string? title { get; set; }
        public virtual string? section { get; set; }
        public virtual List<PersonCoursesDto>? CoursePersons { get;  set; }
        // Other properties you need
    }
}
