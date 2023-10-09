namespace WebApplication1.Models
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PersonDto> People { get; set; }
        // Other properties you need
    }
}
