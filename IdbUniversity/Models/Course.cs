using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdbUniversity.Models
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Course ID")]
        public int CourseID { get; set; }

        [Required]
        [Display(Name = "Course Name"), StringLength(100, MinimumLength = 3)]
        public string CourseTitle { get; set; }

        [Required]
        [Range(0, 200)]
        [Display(Name = "Course Credits")]
        public int CourseCredits { get; set; }

        [Required]
        [Display(Name = "Department ID")]
        public int DepartmentID { get; set; } 

        public virtual Department Department { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Instructor> Instructors { get; set; }
    }
}
