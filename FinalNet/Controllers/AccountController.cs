using FinalNet.Models;
using G6MVCDemo.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalNet.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        CMSDbContexts context;
        public AccountController(CMSDbContexts _CMSDbContexts, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            context = _CMSDbContexts;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("Account/Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Route("Account/Register")]
        public IActionResult Register(Register usermodel)
        {
            if (!ModelState.IsValid)
            {
                return View(usermodel);
            }
            var user = new User { Name = usermodel.FirstName, UserName = usermodel.Email, Email = usermodel.Email, Type = usermodel.Type, PhoneNumber = usermodel.PhoneNumber };
            //var nstudent = new Studentclass { Email = usermodel.Email ,FName = usermodel.FirstName,LName = usermodel.LastName};
            var result = _userManager.CreateAsync(user, usermodel.Password).Result;
            result = _userManager.AddToRoleAsync(user, Enum.GetName(typeof(UserType), usermodel.Type)).Result;
            if (result.Succeeded == false)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError("Error", error.Description);
                }
                return View(usermodel);
            }
            if (usermodel.Type == UserType.Student)
            {
                var student = new Studentclass { Name = usermodel.FirstName, FName = usermodel.FirstName, LName = usermodel.LastName, Email = usermodel.Email, Ph = usermodel.PhoneNumber };
                context.Add(student);
                context.SaveChanges();
                user.UserId = student.Id;
            }
            else
            {
                var instructor = new Instuctorclass { Name = usermodel.FirstName, FName = usermodel.FirstName, LName = usermodel.LastName, Email = usermodel.Email, Ph = usermodel.PhoneNumber };
                context.Add(instructor);
                context.SaveChanges();
                user.UserId = instructor.Id;
            }
            result = _userManager.UpdateAsync(user).Result;

            return RedirectToAction("Login");
        }


        [HttpGet]
        [Route("Account/Login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [Route("Account/Login")]
        public IActionResult Login(Login usermodel)
        {
            if (!ModelState.IsValid)
            {
                return View(usermodel);
            }
            var user = _userManager.FindByEmailAsync(usermodel.Email).Result;
            if (user != null && _userManager.CheckPasswordAsync(user, usermodel.Password).Result == true)
            {
                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                var role = _userManager.GetRolesAsync(user).Result;
                foreach (var item in role)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, item));
                }
                identity.AddClaim(new Claim("userid", user.UserId.ToString()));
                HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
                return RedirectToAction("");
            }
            else
            {
                ModelState.AddModelError("Error", "invalid username or passord");
                return View();
            }
        }

        public IActionResult Instructor()
        {
            var userId = Convert.ToInt32(User.FindFirstValue("userid"));
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var instructor = context.Instuctors
                    .Select(c => new Instuctorclass
                    {
                        Name = c.Name,
                        FName = c.FName,
                        LName = c.LName,
                        Email = c.Email,
                        Ph = c.Ph,
                        Id = c.Id
                    }).ToList();
            ViewBag.UserRole = userRole;
            return View(instructor);

        }
        [Route("Instructor/{id}")]
        public IActionResult InsDetails(int id)
        {
            
            var ins = context.Instuctors.FirstOrDefault(c => c.Id == id);
            if (ins == null)
            {
                return NotFound();
            }
            var instructor = new Instuctorclass
            {
                Name = ins.Name,
                FName = ins.FName,
                LName = ins.LName,
                Email = ins.Email,
                Ph = ins.Ph,
                Id = ins.Id
            };


           
            List<CoursesDetails> courses;
            
                courses = context.Courses
            .Where(c => c.InstructorId == id)
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
            

            var ViewModelMA = new ViewModelMA
            {
                InstuctorclassMA = instructor,
                ListCourseDetailsMA = courses,
            };
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            ViewBag.UserRole = userRole;
            return View(ViewModelMA);
        }
        
        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
