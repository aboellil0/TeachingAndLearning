using FinalNet.Models;
using G6MVCDemo.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;

namespace FinalNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        CMSDbContexts context;
        public HomeController(CMSDbContexts _CMSDbContexts, ILogger<HomeController> logger)
        {
            context = _CMSDbContexts;
            _logger = logger;

        }


        public IActionResult Index()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            ViewBag.UserRole = userRole;
            return View(Index);
        }
        [Authorize(Roles = $"{nameof(UserType.Instructor)},{nameof(UserType.Student)}")]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize(Roles = $"{nameof(UserType.Instructor)},{nameof(UserType.Student)}")]
        public IActionResult Courses()
        {
            var userId = Convert.ToInt32(User.FindFirstValue("userid"));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == nameof(UserType.Instructor))
            {
                var courses = context.Courses
                    .Where(c => c.InstructorId == userId)
                    .Select(c => new CoursesDetails
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        StartDate = c.StartDate,
                        Capacity = c.Capacity,
                        Duration = c.Duration,
                        EndDate = c.EndDate,
                    }).ToList();
                ViewBag.UserRole = userRole;
                return View(courses);
            }
            else
            {
                var courses = (from c in context.Courses
                               where c.Enrollments.Any(en => en.StudentId == userId) == false
                               select new CoursesDetails
                               {
                                   Id = c.Id,
                                   Name = c.Name,
                                   Description = c.Description,
                                   StartDate = c.StartDate,
                                   Capacity = c.Capacity,
                                   Duration = c.Duration,
                                   EndDate = c.EndDate,
                               }).ToList();
                ViewBag.UserRole = userRole;
                return View(courses);
            }
        }
        [Authorize(Roles = nameof(UserType.Student))]
        public ActionResult EnrollmentCourses()
        {
            var userId = Convert.ToInt32(User.FindFirstValue("userid"));
            var courses = (from c in context.Courses
                           where c.Enrollments.Any(en => en.StudentId == userId)
                           select new CoursesDetails
                           {
                               Id = c.Id,
                               Name = c.Name,
                               Description = c.Description,
                               StartDate = c.StartDate,
                               Capacity = c.Capacity,
                               Duration = c.Duration,
                               EndDate = c.EndDate,
                           }).ToList();
            return View(courses);
        }

        [Authorize(Roles = nameof(UserType.Instructor))]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = nameof(UserType.Instructor))]
        [HttpPost]
        public IActionResult CreateCourse(CreateCourses model)
        {
            var userId = Convert.ToInt32(User.FindFirstValue("userid"));
            var state = ModelState;
            if (ModelState.IsValid == false)
            {
                return View("Create", model);
            }
            var course = new Course()
            {
                Name = model.Name,
                Description = model.Description,
                StartDate = model.StartDate,
                Capacity = model.Capacity,
                Duration = model.Duration,
                EndDate = model.EndDate,
                InstructorId = userId,
            };
            context.Courses.Add(course);
            context.SaveChanges();
            return RedirectToAction("Courses");
        }
        [Authorize(Roles = $"{nameof(UserType.Instructor)},{nameof(UserType.Student)}")]
        [Route("Courses/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.Id = id;
            var userId = Convert.ToInt32(User.FindFirstValue("userid"));
            var course = context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            var courseModule = new CoursesDetails
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                StartDate = course.StartDate,
                Capacity = course.Capacity,
                Duration = course.Duration,
                EndDate = course.EndDate,

            };

            //bool isEnrolled = (from c in context.Courses
            //                   join e in context.CourseEnrollments on c.Id equals e.CourseId
            //                   where e.StudentId == userId
            //                   select c
            //    ).Any();

            bool isEnrolled = (from enrollment in context.CourseEnrollments
                               join c in context.Courses on enrollment.CourseId equals c.Id
                               where enrollment.StudentId == userId && c.Id == id
                               select enrollment).Any();

            ViewBag.isEnrolled = isEnrolled;


            var students = (from c in context.Students
                            join e in context.CourseEnrollments on c.Id equals e.StudentId
                            where e.CourseId == id
                            select new Studentclass
                            {
                                Name = c.Name,
                                FName = c.FName,
                                LName = c.LName,
                                Email = c.Email,
                                Ph = c.Ph,
                                Id = c.Id
                            }).ToList();
            var ViewModelMA = new ViewModelMA
            {
                CoursesDetailsMA = courseModule,
                StudentclassMA = students,
            };
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            ViewBag.UserRole = userRole;
            return View(ViewModelMA);
        }
        [Authorize(Roles = nameof(UserType.Student))]
        public IActionResult Enroll(int id)
        {
            var userId = Convert.ToInt32(User.FindFirstValue("userid"));
            var courseEnrollment = new CourseEnrollment { StudentId = userId, CourseId = id };
            context.CourseEnrollments.Add(courseEnrollment);
            context.SaveChanges();
            return RedirectToAction("Courses");
        }
        [Authorize(Roles = nameof(UserType.Instructor))]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var courses = context.Courses.FirstOrDefault(c => c.Id == id);
            if (courses == null)
            {
                return NotFound();
            }
            return View(courses);
        }
        [Authorize(Roles = nameof(UserType.Instructor))]
        [HttpPost]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                var userId = Convert.ToInt32(User.FindFirstValue("userid"));
                context.Courses.Update(course);
                course.InstructorId = userId;
                context.SaveChanges();
                //return View(course);
                return RedirectToAction("Courses");
            }
            return View(course);
        }

        [Authorize(Roles = nameof(UserType.Instructor))]
        public ActionResult Delete(int? id)
        {
            Course course = context.Courses.Find(id);
            return View(course);
        }

        [Authorize(Roles = nameof(UserType.Instructor))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = context.Courses.Find(id);
            if (course != null)
            {
                context.Courses.Remove(course);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }




        [Authorize(Roles = nameof(UserType.Instructor))]
        public ActionResult Unenroll(int? studentId, int? courseId)
        {
            var enrollment = context.CourseEnrollments
                .SingleOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);
            return View(enrollment);
        }
        [Authorize(Roles = nameof(UserType.Instructor))]
        [HttpPost, ActionName("Unenroll")]
        [ValidateAntiForgeryToken]
        public ActionResult UnenrollConfirmed(int studentId, int courseId)
        {
            var enrollment = context.CourseEnrollments
                .SingleOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment != null)
            {
                context.CourseEnrollments.Remove(enrollment);
                context.SaveChanges();
            }

            return RedirectToAction("Index"); 
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}