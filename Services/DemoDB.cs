using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Util;
using System;
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

            if (student.PersonType == PersonType.Teacher)
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
        public Person CreateStudent(Person student)
        {
            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(student);
                    transaction.Commit();
                }

            }
            return student;

        }
        public Course CreateCourse(Course course)
        {
            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(course);
                    transaction.Commit();
                }

            }
            return course;

        }
        public List<Person> GetCurrentStudents()
        {
            List<Person> students = new List<Person>();
            NHibernate.ISession session = _sessionFactory.OpenSession();

            ITransaction transaction = session.BeginTransaction();
  
            students = session.Query<Person>().Where(p => p.PersonType == PersonType.Student).ToList();

            transaction.Commit();


            return students;

        }

        public List<CourseDto> GetCourses()
        {
            List<CourseDto> courses;
            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    courses = session.Query<Course>().Select(c => new CourseDto
                    {
                        Id = c.ID,
                        Name = c.title + c.section,
                        People = new List<PersonDto>()
                        /*People = c.People.Select(per => new PersonDto
                        {
                            Id = per.ID,
                            Name = per.FirstMidName + " " + per.LastName
                            // Map other course properties as needed
                        }).ToList()*/
                    }).ToList();
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
        public List<CourseDto> GetStudentCourse(int pid)
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

                            Id = c.ID,
                            Name = c.title + c.section

                        }
                        ).ToList();

                    transaction.Commit();
                }

            }


            return courses;
        }
        public PersonDto Get(int id)
        {
            PersonDto result;
            NHibernate.ISession session = _sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            var p = session.Get<Person>(id);
            result = new PersonDto
            {
                Id = p.ID,
                Name = p.FirstMidName + " " + p.LastName,
                Courses = new List<CourseDto>()
                
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
                Id = c.ID,
                Name = c.title + " " + c.section,
                People = session.QueryOver<CoursePerson>()
                        .Where(cp => cp.course.ID == id)
                        .Select(cp => cp.person)
                        .List<Person>().Select(p => new PersonDto
                {
                            Id = p.ID,
                            Name = p.FirstMidName + " " + p.LastName,
                            Courses = new List<CourseDto>()
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
