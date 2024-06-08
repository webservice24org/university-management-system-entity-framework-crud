using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace IdbUniversity.Models.ViewModel
{
    public class InstructorViewModel
    {
        public int InstructorID { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Join Date")]
        public DateTime JoinDate { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => FirstName  + " " + LastName;

        [Display(Name = "Instructor's Picture")]
        public string InstructorPicture { get; set; } = "";

        [ValidateFile]
        [Display(Name = "Instructor's Picture")]

        public HttpPostedFileBase PicturePath { get; set; }

        public IEnumerable<Instructor> Instructors { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
        public virtual OfficeAssignment OfficeAssignment { get; set; }

        public IEnumerable<int> SelectedCourseIDs { get; set; } 
        public IEnumerable<Course> AllCourses { get; set; } 
    }
}
