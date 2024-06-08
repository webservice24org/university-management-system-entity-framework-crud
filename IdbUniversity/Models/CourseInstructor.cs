using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdbUniversity.Models
{
    public class CourseInstructor
    {
        public int CourseID { get; set; }
        public int InstructorID { get; set; }

        public virtual Course Course { get; set; }
        public virtual Instructor Instructor { get; set; }
    }

}