using FinalNet.Models;

namespace G6MVCDemo.DAL
{
    public class Course
    { 
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int Duration { get; set; }
            public int Capacity { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int InstructorId { get; set; }
            public List<CourseEnrollment> Enrollments { get; set; } = new();
    }
}
