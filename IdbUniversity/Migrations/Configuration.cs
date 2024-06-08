namespace IdbUniversity.Migrations
{
    using IdbUniversity.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<IdbUniversity.DAL.UniversityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(IdbUniversity.DAL.UniversityContext context)
        {


            context.Departments.AddOrUpdate(
                d => d.DepartmentID,
                new Department { DepartmentID = 1, DepartmentName = "Computer Science", Budget = 350000, StartDate = DateTime.Parse("2020-09-01"), InstructorID = 1 },
                new Department { DepartmentID = 2, DepartmentName = "Mathematics", Budget = 200000, StartDate = DateTime.Parse("2020-09-01"), InstructorID = 2 }
            );


            context.Instructors.AddOrUpdate(
                i => i.InstructorID,
                new Instructor { InstructorID = 1, LastName = "Smith", FirstName = "John", JoinDate = DateTime.Parse("2015-03-11"), InstructorPicture = "" },
                new Instructor { InstructorID = 2, LastName = "Doe", FirstName = "Jane", JoinDate = DateTime.Parse("2016-07-06"), InstructorPicture = "" }
            );


            context.OfficeAssignments.AddOrUpdate(
                oa => oa.InstructorID,
                new OfficeAssignment { InstructorID = 1, Location = "Room 101" },
                new OfficeAssignment { InstructorID = 2, Location = "Room 202" }
            );

            context.Courses.AddOrUpdate(
                c => c.CourseID,
                new Course { CourseID = 1254, CourseTitle = "Algorithms", CourseCredits = 4, DepartmentID = 1 },
                new Course { CourseID = 1258, CourseTitle = "Data Structures", CourseCredits = 3, DepartmentID = 1 },
                new Course { CourseID = 1257, CourseTitle = "Linear Algebra", CourseCredits = 3, DepartmentID = 2 },
                new Course { CourseID = 1256, CourseTitle = "Calculus", CourseCredits = 4, DepartmentID = 2 }
            );


            context.Students.AddOrUpdate(
                s => s.StudentId,
                new Student { StudentId = 1, LastName = "Brown", FirstMidName = "Charlie", Email = "charlie.brown@example.com", Phone = "555-1234", Picture = "", IsActive = true, EnrollmentDate = DateTime.Parse("2022-09-01") },
                new Student { StudentId = 2, LastName = "Johnson", FirstMidName = "Chris", Email = "chris.johnson@example.com", Phone = "555-5678", Picture = "", IsActive = true, EnrollmentDate = DateTime.Parse("2022-09-01") },
                new Student { StudentId = 3, LastName = "Williams", FirstMidName = "Pat", Email = "pat.williams@example.com", Phone = "555-9876", Picture = "", IsActive = true, EnrollmentDate = DateTime.Parse("2022-09-01") },
                new Student { StudentId = 4, LastName = "Taylor", FirstMidName = "Sam", Email = "sam.taylor@example.com", Phone = "555-6543", Picture = "", IsActive = true, EnrollmentDate = DateTime.Parse("2022-09-01") }
            );
            context.SaveChanges();


            context.Enrollments.AddOrUpdate(
                e => e.EnrollmentID,
                new Enrollment { EnrollmentID = 1, StudentId = 1, CourseID = 1254, Grade = Grade.A },
                new Enrollment { EnrollmentID = 2, StudentId = 2, CourseID = 1258, Grade = Grade.B },
                new Enrollment { EnrollmentID = 3, StudentId = 3, CourseID = 1257, Grade = Grade.C },
                new Enrollment { EnrollmentID = 4, StudentId = 4, CourseID = 1256, Grade = Grade.D }
            );
            context.SaveChanges();

            var courseInstructorMapping = new[]
            {
            new { CourseID = 1254, InstructorID = 1 },
            new { CourseID = 1258, InstructorID = 1 },
            new { CourseID = 1257, InstructorID = 2 },
            new { CourseID = 1256, InstructorID = 2 }
        };

            foreach (var mapping in courseInstructorMapping)
            {
                var course = context.Courses.SingleOrDefault(c => c.CourseID == mapping.CourseID);
                var instructor = context.Instructors.SingleOrDefault(i => i.InstructorID == mapping.InstructorID);

                if (course != null && instructor != null)
                {
                    if (!course.Instructors.Contains(instructor))
                    {
                        course.Instructors.Add(instructor);
                    }
                }
                else
                {
                    if (course == null)
                    {
                        Console.WriteLine($"Course with CourseID {mapping.CourseID} not found.");
                    }
                    if (instructor == null)
                    {
                        Console.WriteLine($"Instructor with InstructorID {mapping.InstructorID} not found.");
                    }
                }
            }

            context.SaveChanges();

        }
    }
}
