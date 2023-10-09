namespace WebApplication1.Models
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CourseDto> Courses { get; set; }
        // Other properties you need
    }
}
