namespace WebApplication1.Models
{
    public class CoursePerson
    {
        public virtual int ID { get; set; }
        public virtual Person person { get; set; }
        public virtual Course course { get; set; }
        public virtual LetterGrade? gradeLetter { get; set; }
    }
}
