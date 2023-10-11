namespace WebApplication1.Models
{
    public class Person
    {
        public virtual int ID { get; set; }
        /*
         

This is because NHibernate assigns those guids itself using the guid.comb algorithm, and it doesn't have to rely on the database to do this.

So using the batch size is a great way to tune it.
         */
        public virtual string? LastName { get; set; }
        public virtual string? FirstMidName { get; set; }
        public virtual PersonType PersonType { get; set; }
        public virtual string? Street { get; set; }
        public virtual string? City { get; set; }
        public virtual string? Province { get; set; }
        public virtual string? Country { get; set; }

        public virtual IList<CoursePerson>? CoursePersons { get; set; }

        /*        public Person()
                {
                    Courses = new HashSet<Course>();
                }*/
        /*        protected void addCourse(Course c)
                {
                    if (Courses != null) {  Courses.Add(c); }
                }*/
    }

    public enum PersonType
    {
        Student, //0
        Teacher
    }


}
