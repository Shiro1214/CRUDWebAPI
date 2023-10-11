namespace WebApplication1.Models
{
    public class CourseDto
    {
        public virtual int ID { get; set; }
        public virtual string? title { get; set; }
        public virtual string? section { get; set; }
        public List<PersonDto>? People { get;  set; }
        // Other properties you need
    }
}
