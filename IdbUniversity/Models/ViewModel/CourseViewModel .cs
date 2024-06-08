using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdbUniversity.Models.ViewModel
{
    public class CourseViewModel
    {
        public int CourseID { get; set; }
        public string CourseTitle { get; set; }
        public int CourseCredits { get; set; }
        public int DepartmentID { get; set; }
        public SelectList Departments { get; set; }
    }
}