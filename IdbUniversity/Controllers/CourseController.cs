using IdbUniversity.DAL;
using IdbUniversity.Models;
using IdbUniversity.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

namespace IdbUniversity.Controllers
{
    public class CourseController : Controller
    {
        private UniversityContext db = new UniversityContext();
        public ActionResult Index()
        {
            var courses = db.Courses.Include(c=>c.Department).ToList();
            return View(courses);
        }

        public ActionResult Create()
        {
            PopulateDepartmentDropdownList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID, CourseTitle, CourseCredits, DepartmentID")] Course course)
        {
            try
            {
                if (ModelState.IsValid == true)
                {
                    db.Courses.Add(course);
                    var a = db.SaveChanges();

                    if (a > 0)
                    {
                        TempData["SuccessMessage"] = "Course Added";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "<script>alert('Course Did not Add!')<script>";
                    }
                }
            }
            catch (RetryLimitExceededException) 
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDepartmentDropdownList(course.DepartmentID);
            return View(course);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            var departments = db.Departments.Select(d => new
            {
                d.DepartmentID,
                d.DepartmentName
            }).ToList();

            var viewModel = new CourseViewModel
            {
                CourseID = course.CourseID,
                CourseTitle = course.CourseTitle,
                CourseCredits = course.CourseCredits,
                DepartmentID = course.DepartmentID,
                Departments = new SelectList(departments, "DepartmentID", "DepartmentName", course.DepartmentID)
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseID,CourseTitle,CourseCredits,DepartmentID")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDepartmentDropdownList(course.DepartmentID);
            return View(course);
        }

        private void PopulateDepartmentDropdownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in db.Departments
                                   orderby d.DepartmentName
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "DepartmentName", selectedDepartment);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }
    }
}