namespace WebApplication1.Models
{
    public class Grade
    {

        public virtual int ID { get; set; }
        public virtual LetterGrade? letter { get; set; }
        public virtual int course_id { get; set; }
        public virtual int person_id { get; set; }

    }

    public enum LetterGrade
    {
        A, B, C, D, E, F
    }
}
