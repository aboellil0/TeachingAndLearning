using G6MVCDemo.DAL;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace FinalNet.Models
{
    public class Class
    {
    }
    public class CoursesDetails
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Required")]
        public int Capacity { get; set; }
        [Required(ErrorMessage = "Required")]
        public int Duration { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime EndDate { get; set; }
       
    }
    public class CreateCourses 
    {
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Required")]
        public int Capacity { get; set; }
        [Required(ErrorMessage = "Required")]
        public int Duration { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime EndDate { get; set; }
    }
    public class Register
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
            set
            {
                FullName = FirstName + " " + LastName;
            }
        }
        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Required")]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password donot match")]
        public string PasswordConfirmed { get; set; }
        [Required(ErrorMessage = "Required")]
        public UserType Type { get; set; }
    }
    public class Login
    {
        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class EnrollModule
    {

        public int StudentId { get; set; }
        public int CourseId { get; set; }
    }
    public class User : IdentityUser<int>
    {
       
        public string Name { get; set; }
        public int UserId { get; set; }
        public UserType Type { get; internal set; }
    }
    public enum UserType
    {
        Student,
        Instructor
    }

    public class Role : IdentityRole<int>
    {

    }
    public class Instuctorclass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Ph { get; set; }

    }
    public class Studentclass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Ph { get; set; }
    }
    public class CourseEnrollment
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
    }
}
