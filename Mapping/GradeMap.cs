using FluentNHibernate.Mapping;
using WebApplication1.Models;

namespace WebApplication1.Mapping
{
    public class GradeMap : ClassMap<Grade>
    {
        public GradeMap()
        {
            Table("grade");
            Id(x => x.ID).GeneratedBy.Increment();
            Map(x => x.letter);
            Map(x => x.course_id);
            Map(x => x.person_id);
        }
    }
}
