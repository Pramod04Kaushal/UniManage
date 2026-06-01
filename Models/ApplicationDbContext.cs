using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace UniManage.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Lecturer> Lecturers { get; set; }

        public DbSet<Module> Modules { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseModule> CourseModules { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }

        public DbSet<Grade> Grades { get; set; }
    }
}