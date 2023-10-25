namespace WebApplication1.Models
{
    public class PersonCoursesDto
    {
        public virtual int ID { get; set; }
        public virtual PersonDto Teacher { get; set; }
        public virtual CourseDto course { get; set; }
        public virtual string gradeLetter { get; set; }
    }
}
