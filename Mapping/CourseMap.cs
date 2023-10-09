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

            HasManyToMany(x => x.People)
                .Table("course_person")
                .ParentKeyColumn("course_id")
                .ChildKeyColumn("person_id")
                .Cascade.All()
                .Inverse();
            //HasManyToMany(x => x.People).Inverse().Cascade.All().Table("course_person");
        }

    }
}
