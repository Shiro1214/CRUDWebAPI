using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
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
            if (student.Courses != null)
            {
                student.Courses.Add(course);
            } else
            {
                student.Courses = new HashSet<Course>
                {
                    course
                };
            }

            if (course.People != null)
            {
                course.People.Add(student);
            }
            else
            {
                course.People = new HashSet<Person>
                {
                    student
                };
            }

            /*            student.addCourse(course);
                        course.addPerson(student);*/

            session.SaveOrUpdate(student);
            session.SaveOrUpdate(course);

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

        public List<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();
            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    courses = session.Query<Course>().ToList();
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

        public List<Course> GetStudentCourse(int pid)
        {
            List<Course> courses = new List<Course>();

            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    //var p = session.Get<Person>(pid);

                    Person student = session.QueryOver<Person>()
                        .Where(e => e.ID == pid)
                        .JoinQueryOver(e => e.Courses)
                        .SingleOrDefault();

                    if (student != null)
                    {
                        courses.AddRange(student.Courses);
                    }

 /*                   //return p.Courses
                    if (p.Courses != null)
                    {
                        courses = p.Courses.ToList();
                    }*/
                }

            }
            /*
                        foreach (var c in courses)
                        {
                            Console.WriteLine(c.title);
                        }*/

            return courses;
        }
        public Person Get(int id)
        {
            Person result;
            NHibernate.ISession session = _sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            result = session.Get<Person>(id);
            transaction.Commit();
            /*            using (NHibernate.ISession session = _sessionFactory.OpenSession())
                        {
                            using (ITransaction transaction = session.BeginTransaction())
                            { 
                                result = session.Get<Person>(id);
                                transaction.Commit();
                            }
                        }*/
            return result;
        }
        public bool Update(int id, Person updated)
        {
            Person cur = Get(id);
            if (cur == null)
            {
                return false;
            }
            cur.FirstMidName = updated.FirstMidName;
            cur.LastName    = updated.LastName;
            cur.PersonType = updated.PersonType;
            cur.Street  =   updated.Street;
            cur.City = updated.City;
            cur.Country = updated.Country;
            cur.Province    = updated.Province;
            cur.Courses = updated.Courses;

            using (NHibernate.ISession session = _sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
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

    }
}
