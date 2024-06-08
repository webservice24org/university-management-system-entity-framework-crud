using IdbUniversity.DAL;
using IdbUniversity.Models.ViewModel;
using IdbUniversity.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System;
using System.Data.Entity;

namespace IdbUniversity.Controllers
{
    public class InstructorController : Controller
    {
        private UniversityContext db = new UniversityContext();
        public ActionResult Index(int? id, int? CourseID)
        {
            var viewModel = new InstructorViewModel();
            viewModel.Instructors = db.Instructors
                .Include("OfficeAssignment")
                .Include("Courses.Department")
                .OrderBy(i => i.LastName)
                .ToList();

            if (id != null)
            {
                ViewBag.InstructorID = id.Value;
                viewModel.Courses = viewModel.Instructors.Where(i => i.InstructorID == id.Value).Single().Courses;
            }

            if (CourseID != null)
            {
                ViewBag.CourseID = CourseID.Value;
                viewModel.Enrollments = viewModel.Courses.Where(x => x.CourseID==CourseID).Single().Enrollments;
            }
            return View(viewModel); 
        }

        public ActionResult Create()
        {
            var viewModel = new InstructorViewModel
            {
                AllCourses = db.Courses.ToList(),
                OfficeAssignment = new OfficeAssignment()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstructorViewModel viewModel, int[] SelectedCourseIDs)
        {
            if (ModelState.IsValid)
            {
                var instructor = new Instructor
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    JoinDate = viewModel.JoinDate,
                    InstructorPicture = viewModel.InstructorPicture
                };

                if (!string.IsNullOrWhiteSpace(viewModel.OfficeAssignment.Location))
                {
                    instructor.OfficeAssignment = new OfficeAssignment
                    {
                        Location = viewModel.OfficeAssignment.Location
                    };
                }

                if (viewModel.PicturePath != null && viewModel.PicturePath.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(viewModel.PicturePath.FileName);
                    string extension = Path.GetExtension(viewModel.PicturePath.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Server.MapPath("~/images/"), fileName);
                    viewModel.PicturePath.SaveAs(path);
                    instructor.InstructorPicture = "~/images/" + fileName;
                }

                if (SelectedCourseIDs != null)
                {
                    instructor.Courses = new List<Course>();
                    foreach (var courseId in SelectedCourseIDs)
                    {
                        var course = db.Courses.Find(courseId);
                        instructor.Courses.Add(course);
                    }
                }

                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            viewModel.AllCourses = db.Courses.ToList();
            return View(viewModel);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Instructor instructor = db.Instructors
                .Include("OfficeAssignment")

                .Include("Courses")
                                                  
                .SingleOrDefault(i => i.InstructorID == id);
            if (instructor == null)
            {
                return HttpNotFound();
            }

            var viewModel = new InstructorViewModel
            {
                InstructorID = instructor.InstructorID, 
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                JoinDate = instructor.JoinDate,
                InstructorPicture = instructor.InstructorPicture,
                OfficeAssignment = new OfficeAssignment
                {
                    Location = instructor.OfficeAssignment?.Location
                },

                SelectedCourseIDs = instructor.Courses.Select(c => c.CourseID).ToList(),
                AllCourses = db.Courses.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InstructorViewModel viewModel, int[] SelectedCourseIDs)
        {
            
            var instructorToUpdate = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Single(i => i.InstructorID == viewModel.InstructorID);

            instructorToUpdate.InstructorID = viewModel.InstructorID;
            instructorToUpdate.FirstName = viewModel.FirstName;
            instructorToUpdate.LastName = viewModel.LastName;
            instructorToUpdate.JoinDate = viewModel.JoinDate;

            if (string.IsNullOrWhiteSpace(viewModel.OfficeAssignment?.Location))
            {
                if (instructorToUpdate.OfficeAssignment != null)
                {
                    db.OfficeAssignments.Remove(instructorToUpdate.OfficeAssignment);
                }
            }
            else
            {
                if (instructorToUpdate.OfficeAssignment != null)
                {
                    instructorToUpdate.OfficeAssignment.Location = viewModel.OfficeAssignment.Location;
                }
                else
                {
                    instructorToUpdate.OfficeAssignment = new OfficeAssignment
                    {
                        InstructorID = instructorToUpdate.InstructorID,
                        Location = viewModel.OfficeAssignment.Location
                    };
                    db.OfficeAssignments.Add(instructorToUpdate.OfficeAssignment);
                }
            }

            if (viewModel.PicturePath != null && viewModel.PicturePath.ContentLength > 0)
            {

                if (!string.IsNullOrEmpty(instructorToUpdate.InstructorPicture))
                {
                    string oldImagePath = Server.MapPath(instructorToUpdate.InstructorPicture);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string fileName = Path.GetFileNameWithoutExtension(viewModel.PicturePath.FileName);
                string extension = Path.GetExtension(viewModel.PicturePath.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(Server.MapPath("~/images/"), fileName);
                viewModel.PicturePath.SaveAs(path);
                instructorToUpdate.InstructorPicture = "~/images/" + fileName;
            }
            else
            {
                instructorToUpdate.InstructorPicture = viewModel.InstructorPicture;
            }

            var selectedCoursesHS = new HashSet<int>(SelectedCourseIDs ?? new int[] { });
            var instructorCoursesHS = new HashSet<int>(instructorToUpdate.Courses.Select(c => c.CourseID));

            foreach (var course in db.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID))
                {
                    if (!instructorCoursesHS.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(course);
                    }
                }
                else
                {
                    if (instructorCoursesHS.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Remove(course);
                    }
                }
            }

            db.Entry(instructorToUpdate).State = EntityState.Modified;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Instructor updated successfully.";
            return RedirectToAction("Index");
            

            viewModel.AllCourses = db.Courses.ToList();
            return View(viewModel);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Instructor instructor = db.Instructors
                .Include("OfficeAssignment")

                .Include("Courses")

                .SingleOrDefault(i => i.InstructorID == id);
            if (instructor == null)
            {
                return HttpNotFound();
            }

            var viewModel = new InstructorViewModel
            {
                InstructorID = instructor.InstructorID,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                JoinDate = instructor.JoinDate,
                InstructorPicture = instructor.InstructorPicture,
                OfficeAssignment = new OfficeAssignment
                {
                    Location = instructor.OfficeAssignment?.Location
                },

                SelectedCourseIDs = instructor.Courses.Select(c => c.CourseID).ToList(),
                AllCourses = db.Courses.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var instructorToDelete = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .SingleOrDefault(i => i.InstructorID == id);

            if (instructorToDelete != null)
            {
                if (instructorToDelete.OfficeAssignment != null)
                {
                    db.OfficeAssignments.Remove(instructorToDelete.OfficeAssignment);
                }

                instructorToDelete.Courses.Clear();

                if (!string.IsNullOrEmpty(instructorToDelete.InstructorPicture))
                {
                    string imagePath = Server.MapPath(instructorToDelete.InstructorPicture);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                var departmentsToUpdate = db.Departments.Where(d => d.InstructorID == id).ToList();
                foreach (var department in departmentsToUpdate)
                {
                    department.InstructorID = null; 
                }

                db.Instructors.Remove(instructorToDelete);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Instructor deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Instructor not found.";
            }

            return RedirectToAction("Index");
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(c => c.Department)) 
                .SingleOrDefault(i => i.InstructorID == id);

            if (instructor == null)
            {
                return HttpNotFound();
            }

            var viewModel = new InstructorViewModel
            {
                InstructorID = instructor.InstructorID,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                JoinDate = instructor.JoinDate,
                InstructorPicture = instructor.InstructorPicture,
                OfficeAssignment = new OfficeAssignment
                {
                    Location = instructor.OfficeAssignment?.Location
                },
                SelectedCourseIDs = instructor.Courses.Select(c => c.CourseID).ToList(),
                AllCourses = db.Courses.ToList(),
                Courses = instructor.Courses.ToList() 
            };

            return View(viewModel);
        }







    }
}