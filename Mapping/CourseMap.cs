using FluentNHibernate.Mapping;
using WebApplication1.Models;

namespace WebApplication1.Mapping
{
    public class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            Table("course");
            Id(x => x.ID).GeneratedBy.Increment();
            Map(x => x.title);
            Map(x => x.section);
            HasMany(x => x.CoursePersons)
            .Cascade.AllDeleteOrphan()
            .Inverse()
            .KeyColumn("course_id");
        }

    }
}
