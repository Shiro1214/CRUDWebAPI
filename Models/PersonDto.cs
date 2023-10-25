using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class PersonDto
    {
        public int Id { get; set; }
        public virtual string? LastName { get; set; }
        public virtual string? FirstMidName { get; set; }
        public virtual string? PersonType { get; set; }
        public virtual string? Street { get; set; }
        public virtual string? City { get; set; }
        public virtual string? Province { get; set; }
        public virtual string? Country { get; set; }
        public List<CourseDto>? Courses { get; set; }
        // Other properties you need
    }
}
