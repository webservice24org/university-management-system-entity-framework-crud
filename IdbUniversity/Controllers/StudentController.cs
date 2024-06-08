using IdbUniversity.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IdbUniversity.Models;
using IdbUniversity.Models.ViewModel;
using System.Data.Entity;
using System.Net;
using PagedList;
using System.Web.Services.Description;
using System.Web.UI.WebControls;

namespace IdbUniversity.Controllers
{
    public class StudentController : Controller
    {
        public UniversityContext db = new UniversityContext();

        public ActionResult Index(string SortOrder, string SearchString, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = SortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(SortOrder) ? "name_desc" : "";
            ViewBag.FirstNameSortParam = SortOrder == "FirstMidName" ? "first_desc" : "FirstMidName";
            ViewBag.EmailSortParam = SortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.PhoneSortParam = SortOrder == "Phone" ? "phone_desc" : "Phone";
            ViewBag.DateSortParam = SortOrder == "Date" ? "date_desc" : "Date";

            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            ViewBag.CurrentFilter = SearchString;

            var students = from s in db.Students select s;

            if (!String.IsNullOrEmpty(SearchString))
            {
                students = students.Where(s => s.LastName.ToUpper().Contains(SearchString.ToUpper())
                                            || s.FirstMidName.ToUpper().Contains(SearchString.ToUpper()));
            }

            switch (SortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "FirstMidName":
                    students = students.OrderBy(s => s.FirstMidName);
                    break;
                case "first_desc":
                    students = students.OrderByDescending(s => s.FirstMidName);
                    break;
                case "Email":
                    students = students.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    students = students.OrderByDescending(s => s.Email);
                    break;
                case "Phone":
                    students = students.OrderBy(s => s.Phone);
                    break;
                case "phone_desc":
                    students = students.OrderByDescending(s => s.Phone);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(students.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Create()
        {
            return View();
        }

        public ActionResult AddNewCourse(int? id)
        {
            ViewBag.courses = new SelectList(db.Courses.ToList(), "CourseID", "CourseTitle", (id != null) ? id.ToString() : null);
            return PartialView("_AddNewCourse");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentViewModel vObj, int[] courseId)
        {
            if (ModelState.IsValid)
            {
                var existingEmail = db.Students.FirstOrDefault(s => s.Email == vObj.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(vObj); 
                }

                var existingPhone = db.Students.FirstOrDefault(s => s.Phone == vObj.Phone);
                if (existingPhone != null)
                {
                    ModelState.AddModelError("Phone", "Phone number already exists.");
                    return View(vObj); 
                }

                Student student = new Student
                {
                    LastName = vObj.LastName,
                    FirstMidName = vObj.FirstMidName,
                    Email = vObj.Email,
                    Phone = vObj.Phone,
                    EnrollmentDate = vObj.EnrollmentDate,
                    IsActive = vObj.IsActive
                };

                HttpPostedFileBase file = vObj.PicturePath;
                if (file != null && file.ContentLength > 0)
                {
                    string filepath = Path.Combine("/images/", Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
                    file.SaveAs(Server.MapPath(filepath));
                    student.Picture = filepath;
                }

                db.Students.Add(student);

                foreach (var item in courseId)
                {
                    Enrollment course = new Enrollment
                    {
                        Student = student,
                        StudentId = student.StudentId,
                        CourseID = item
                    };
                    db.Enrollments.Add(course);
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = "Student created successfully.";
                return RedirectToAction("Index");
            }
            return View(vObj);
        }



        [HttpGet]
        public ActionResult Edit(int? id)
        {
            Student student = db.Students.First(x => x.StudentId == id);
            var courses = db.Enrollments.Where(x => x.StudentId == id).ToList();
            StudentViewModel vObj = new StudentViewModel()
            {
                LastName = student.LastName,
                FirstMidName = student.FirstMidName,
                Email = student.Email,
                Phone = student.Phone,
                EnrollmentDate = student.EnrollmentDate,
                IsActive = student.IsActive,
                Picture = student.Picture,
                StudentId=student.StudentId,
            };
            if (courses.Count > 0)
            {
                foreach (var item in courses)
                {
                    vObj.CourseList.Add(item.CourseID);
                }
            }
            return View(vObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentViewModel vObj, int[] courseId)
        {
            
            var existingEmail = db.Students.FirstOrDefault(s => s.Email == vObj.Email && s.StudentId != vObj.StudentId);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(vObj); 
            }

            var existingPhone = db.Students.FirstOrDefault(s => s.Phone == vObj.Phone && s.StudentId != vObj.StudentId);
            if (existingPhone != null)
            {
                ModelState.AddModelError("Phone", "Phone number already exists.");
                return View(vObj); 
            }

            Student student = new Student
            {
                LastName = vObj.LastName,
                FirstMidName = vObj.FirstMidName,
                Email = vObj.Email,
                Phone = vObj.Phone,
                EnrollmentDate = vObj.EnrollmentDate,
                IsActive = vObj.IsActive,
                StudentId = vObj.StudentId
            };

            HttpPostedFileBase file = vObj.PicturePath;
            if (file != null && file.ContentLength > 0)
            {

                if (!string.IsNullOrEmpty(vObj.Picture))
                {
                    string imagePath = Server.MapPath(vObj.Picture);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                string newFilePath = Path.Combine("/images/", Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
                file.SaveAs(Server.MapPath(newFilePath));
                student.Picture = newFilePath;
            }
            else
            {
                student.Picture = vObj.Picture;
            }

            var existingCourses = db.Enrollments.Where(x => x.StudentId == student.StudentId).ToList();
            foreach (var item in existingCourses)
            {
                db.Enrollments.Remove(item);
            }

            foreach (var item in courseId)
            {
                Enrollment enrollment = new Enrollment
                {
                    StudentId = student.StudentId,
                    CourseID = item
                };
                db.Enrollments.Add(enrollment);
            }

            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var student = db.Students.Include(s => s.Enrollments.Select(e => e.Course))
                                     .SingleOrDefault(s => s.StudentId == id);

            if (student == null)
            {
                return HttpNotFound();
            }

            var studentViewModel = new StudentViewModel
            {
                StudentId = student.StudentId,
                LastName = student.LastName,
                FirstMidName = student.FirstMidName,
                Email = student.Email,
                Phone = student.Phone,
                Picture = student.Picture,
                IsActive = student.IsActive,
                EnrollmentDate = student.EnrollmentDate,
                EnrolledCourses = student.Enrollments.Select(e => e.Course).ToList()
            };

            return View(studentViewModel);
        }


        public ActionResult Delete(int? id)
        {
            Student student = db.Students.Find(id);
            if (student != null)
            {

                var existingCourses = db.Enrollments.Where(x => x.StudentId == id).ToList();
                foreach (var item in existingCourses)
                {
                    db.Enrollments.Remove(item);
                }


                if (!string.IsNullOrEmpty(student.Picture))
                {
                    string imagePath = Server.MapPath(student.Picture);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                db.Entry(student).State = EntityState.Deleted;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


    }
}

