namespace WebApplication1.Models
{
    public class Course

    {
        public virtual int ID { get; set; }
        public virtual string? title { get; set; }
        public virtual string? section { get; set; }
        public virtual IList<CoursePerson>? CoursePersons { get; set; }

        /*        public Course() { 
                    People = new HashSet<Person>(); 
                }*/
        /*        protected void addPerson(Person person)
                {
                    if (People != null) People.Add(person);

                }*/

    }

}
