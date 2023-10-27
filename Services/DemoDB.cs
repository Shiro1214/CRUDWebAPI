using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Utils;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Util;
using System;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Transactions;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class DemoDB
    {
        //private readonly ILogger<DemoDB> _logger;
        private readonly string demoDBConnectionString;
        private ISessionFactory _sessionFactory;
       
        public DemoDB()
        {
            //_logger = logger;
            
            try
            {
                demoDBConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NHibernateDemoDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                _sessionFactory = Fluently.Configure().Database(MsSqlConfiguration.MsSql2008.ConnectionString(
            demoDBConnectionString).ShowSql())
             .Mappings(m => m.FluentMappings
             .AddFromAssemblyOf<Program>())
             .ExposeConfiguration(cfg => new SchemaExport(cfg)
             .Create(true, false))
             .BuildSessionFactory();

                }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "DB Environment Setup Incorrectly");
                Environment.Exit(1);
            }


        }

        public bool ChangeStudentGrade(int stuId, int courseId, LetterGrade grade)
        {

            NHibernate.ISession session = _sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            var cur = session.Query<CoursePerson>().Where(cp => cp.person.ID == stuId && cp.course.ID == courseId).SingleOrDefault();

            if (cur == null) { return false; }

            cur.gradeLetter = grade;
            session.SaveOrUpdate(cur);

            transaction.Commit();

            return true;
        }
        public bool AddStudentToCourse(int stuId, int courseId)
        {
            
            NHibernate.ISession session = _sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            var student = session.Get<Person>(stuId);

            if (student == null)
            {
                return false;
            }

            var course = session.Get<Course>(courseId);

            if (course == null)
            {
                return false;
            }

            var cp = new CoursePerson
            {
                person = student,
                course = course,
                gradeLetter = null
            };

            session.SaveOrUpdate(cp);

            transaction.Commit();

            return true;
        }
        public Person CreatePerson(PersonDto person)
        {
            var newStudent = new Person
            {
                ID = person.Id,
                LastName = person.LastName,
                FirstMidName = person.FirstMidName,
                PersonType = person.PersonType.ToLower() == "student" ? PersonType.Student : PersonType.Teacher,
                Street = person.Street,
                City = person.City,
                Province = person.Province,
                Country = person.Country,
                CoursePersons = null
            };
            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    
                    session.Save(newStudent);
                    transaction.Commit();
                }

            }
            return newStudent;

        }
        public Course CreateCourse(CourseDto course)
        {
            var newCourse = new Course
            {
                ID = course.ID,
                title = course.title,
                section = course.section,
                CoursePersons = null
            };
            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(newCourse);
                    transaction.Commit();
                }

            }
            return newCourse;

        }
        public List<PersonDto> GetCurrentPeopleOfType(PersonType pType)
        {
            List<PersonDto> students;
            NHibernate.ISession session = _sessionFactory.OpenSession();

            ITransaction transaction = session.BeginTransaction();
  
            students = session.Query<Person>().Where(p => p.PersonType == pType).ToList().Select(student => new PersonDto {
                Id = student.ID,
                FirstMidName = student.FirstMidName,
                LastName = student.LastName,
                PersonType = student.PersonType == PersonType.Student ? "Student" : "Teacher",
                Street = student.Street,
                City = student.City,
                Country = student.Country,
                Province = student.Province
        }).ToList();

            transaction.Commit();


            return students;

        }

        //Add teacher to result
        public List<CourseDto> GetCourses()
        {
            List<CourseDto> courses;
            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    courses = session.Query<Course>().Select(c => new CourseDto
                    {
                        ID = c.ID,
                        title = c.title, section= c.section
 
                    }).ToList();

                    foreach (var c in courses)
                    {
                        var cp = session.QueryOver<CoursePerson>().Where(cp => cp.course.ID == c.ID).JoinQueryOver(cp => cp.person).Where(p => p.PersonType == PersonType.Teacher).Take(1).SingleOrDefault();
                        if (cp != null) {
                            c.CoursePersons = new List<PersonCoursesDto>
                            {
                                new PersonCoursesDto
                                {
                                    ID = cp.ID,
                                    person = new PersonDto
                                    {
                                        Id = cp.person.ID,
                                        FirstMidName = cp.person.FirstMidName,
                                        LastName = cp.person.LastName
                                    }
                                }
                            };
                        }
                        
                    }
                    transaction.Commit();
                }

            }

/*            foreach (var c in courses)
            {
                Console.WriteLine(c.title);
            }
*/
            return courses;
        }
        public LetterGrade? GetStudentGrade(int pid, int cid)
        {
            LetterGrade? g;

            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    //var p = session.Get<Person>(pid);

                    var coursePerson = session.QueryOver<CoursePerson>()
                       .Where(cp => cp.person.ID == pid && cp.course.ID == cid).SingleOrDefault();

                    g = coursePerson.gradeLetter;

                    transaction.Commit();
                }

            }


            return g;
        }
        public List<CourseDto> GetPersonCourses(int pid)
        {
            List<CourseDto> courses = new List<CourseDto>();

            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    //var p = session.Get<Person>(pid);

                     courses = session.QueryOver<CoursePerson>()
                        .Where(cp => cp.person.ID == pid)
                        .Select(cp => cp.course)
                        .List<Course>().Select(c => new CourseDto {

                            ID = c.ID,
                            title = c.title,
                            section = c.section

                        }
                        ).ToList();

                    transaction.Commit();
                }

            }


            return courses;
        }
        public List<PersonCoursesDto> GetPersonCoursesGrade(int pid)
        {
            List<PersonCoursesDto> personCourses = new List<PersonCoursesDto>();

            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    //var p = session.Get<Person>(pid);
                    personCourses = session.QueryOver<CoursePerson>().Where(cp => cp.person.ID == pid).List<CoursePerson>().Select(cp => new PersonCoursesDto
                    {
                        ID = cp.ID,
                        course = new CourseDto()
                        {
                            ID = cp.course.ID,
                            title = cp.course.title,
                            section = cp.course.section
                        },
                        gradeLetter = cp.gradeLetter switch
                        {
                            LetterGrade.A => "A",
                            LetterGrade.B => "B",
                            LetterGrade.C => "C",
                            LetterGrade.D => "D",
                            LetterGrade.E => "E",
                            _ => "F"
                        }
                    }).ToList();

                    foreach (var pc in  personCourses)
                    {
                        var cp = session.QueryOver<CoursePerson>().Where(cp => cp.course.ID == pc.course.ID).JoinQueryOver(cp => cp.person).Where(p => p.PersonType == PersonType.Teacher).Take(1).SingleOrDefault();
                        if (cp!= null)
                        {
                            var teacher = cp.person;
                            pc.person = new PersonDto
                            {
                                Id = teacher.ID,
                                FirstMidName = teacher.FirstMidName,
                                LastName = teacher.LastName,
                                Courses = new List<CourseDto>(),
                                PersonType = teacher.PersonType == PersonType.Student ? "Student" : "Teacher",
                                Country = teacher.Country,
                                Province = teacher.Province,
                                Street = teacher.Street,
                                City = teacher.City
                            };
                        }

                    }
                    transaction.Commit();
                }

            }


            return personCourses;
        }
        public PersonDto? Get(int id)
        {
            PersonDto? result = null;
            NHibernate.ISession session = _sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            var p = session.Get<Person>(id);
            if  (p!= null) {
                result = new PersonDto
                {
                    Id = p.ID,
                    FirstMidName = p.FirstMidName,
                    LastName = p.LastName,
                    Courses = new List<CourseDto>(),
                    PersonType = p.PersonType == PersonType.Student ? "Student" : "Teacher",
                    Country = p.Country,
                    Province = p.Province,
                    Street = p.Street,
                    City = p.City


                    /*session.QueryOver<CoursePerson>()
                            .Where(cp => cp.person.ID == id)
                            .Select(cp => cp.course)
                            .List<Course>().Select(c => new CourseDto
                            {

                                Id = c.ID,
                                Name = c.title + c.section

                            }
                            ).ToList()*/

                };
            }
                
           
            transaction.Commit();

            return result;
        }
        public CourseDto GetCourse(int id)
        {
            CourseDto result;
            NHibernate.ISession session = _sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            var c = session.Get<Course>(id);
            result = new CourseDto
            {
                ID = c.ID,
                title = c.title,
                section = c.section,
                CoursePersons = session.QueryOver<CoursePerson>()
                        .Where(cp => cp.course.ID == id)
                        .List<CoursePerson>().Select(cp => new PersonCoursesDto
                        {
                            ID = cp.ID,
                            person = new PersonDto
                            {
                                Id = cp.person.ID,
                                FirstMidName = cp.person.FirstMidName,
                                LastName = cp.person.LastName,
                                Courses = new List<CourseDto>(),
                                PersonType = cp.person.PersonType == PersonType.Student ? "Student" : "Teacher",
                                Country = cp.person.Country,
                                Province = cp.person.Province,
                                Street = cp.person.Street,
                                City = cp.person.City
                            },
                            gradeLetter = cp.gradeLetter switch
                            {
                                LetterGrade.A => "A",
                                LetterGrade.B => "B",
                                LetterGrade.C => "C",
                                LetterGrade.D => "D",
                                LetterGrade.E => "E",
                                _ => "F"
                            }
                        }).ToList()
        };
            transaction.Commit();
            return result;
        }
        public bool Update(int id, Person updated)
        {

            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        var cur = session.Get<Person>(id);
                        if (cur == null)
                        {
                            return false;
                        }
                        cur.FirstMidName = updated.FirstMidName;
                        cur.LastName = updated.LastName;
                        cur.PersonType = updated.PersonType;
                        cur.Street = updated.Street;
                        cur.City = updated.City;
                        cur.Country = updated.Country;
                        cur.Province = updated.Province;
                        session.Update(cur);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }

                }
            }
        }
        public void DeleteStudent(int id)
        {
            Person personDelete;
            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    personDelete = session.Get<Person>(id);
                    if (personDelete != null)
                    {
                        session.Delete(personDelete);
                        transaction.Commit();
                    }
                }

            }
            
        }

        public void DeleteCourse(int id)
        {
            Course courseDelete;
            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    courseDelete = session.Get<Course>(id);
                    if (courseDelete != null)
                    {

                        session.Delete(courseDelete);
                        transaction.Commit();
                    }
                }

            }
        }

    }
}
