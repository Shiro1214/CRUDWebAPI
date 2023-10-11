using FluentNHibernate.Mapping;
using WebApplication1.Models;

namespace WebApplication1.Mapping
{
    public class CoursePersonMapping : ClassMap<CoursePerson>
    {
        public CoursePersonMapping()
        {
            Table("course_person");
            Id(x => x.ID).GeneratedBy.Increment();
            References(x => x.person, "person_id").Cascade.None();
            References(x => x.course, "course_id").Cascade.None();
            Map(x => x.gradeLetter).Nullable();

        }
    }
}
