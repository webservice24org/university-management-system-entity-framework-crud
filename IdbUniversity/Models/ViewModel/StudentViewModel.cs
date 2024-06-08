using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace IdbUniversity.Models.ViewModel
{
    public partial class StudentViewModel
    {
        public StudentViewModel()
        {
            this.CourseList = new List<int>();
            this.EnrolledCourses = new List<Course>();
        }

        public int StudentId { get; set; }

        [Required, StringLength(20), Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required, StringLength(20), Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        [EmailAddress, Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required, StringLength(14, ErrorMessage = "Phone number cannot be longer than 14 characters.")]
        [RegularExpression(@"^\d{10,14}$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        public string Picture { get; set; }

        [Display(Name = "Active?")]
        public bool IsActive { get; set; }

        [Required, Display(Name = "Enrollment Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EnrollmentDate { get; set; }

        [ValidateFile]
        public HttpPostedFileBase PicturePath { get; set; }

        

        public List<int> CourseList { get; set; }

        public List<Course> EnrolledCourses { get; set; }
    }
}
