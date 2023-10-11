using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Mapping
{
    public class PersonMap : ClassMap<Person>
    {
        public PersonMap()
        {
            Table("Person");

            Id(x => x.ID).GeneratedBy.Increment();
            Map(x => x.FirstMidName);
            Map(x => x.LastName);
            Map(x => x.PersonType);
            Map(x => x.Street);
            Map(x => x.City);
            Map(x => x.Province);
            Map(x => x.Country);
            HasMany(x => x.CoursePersons)
            .Cascade.AllDeleteOrphan()
            .Inverse()
            .KeyColumn("person_id");
            // HasManyToMany(x => x.Courses).Cascade.All().Table("course_person");
        }
    }
}
