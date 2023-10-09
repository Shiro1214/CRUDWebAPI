namespace WebApplication1.Models
{
    public class CoursePerson
    {
        public virtual int person_id { get; set; }
        public virtual int course_id { get; set; }
        public virtual LetterGrade? gradeLetter { get; set; }
    }
}
