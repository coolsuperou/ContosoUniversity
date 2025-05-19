using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
    public class CoursesController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Courses
        public ActionResult Index()
        {
            var courses = db.Courses.Include("Enrollments").ToList();
            return View(courses);
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,Description,ImageFile")] Course course)
        {
            if (ModelState.IsValid)
            {
                // 处理图片上传
                if (course.ImageFile != null && course.ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(course.ImageFile.FileName);
                    var uploadPath = Path.Combine(Server.MapPath("~/Uploads/CourseImages"), fileName);
                    course.ImageFile.SaveAs(uploadPath);
                    course.ImageUrl = "/Uploads/CourseImages/" + fileName;
                }

                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseID,Title,Credits,Description,ImageUrl,ImageFile")] Course course)
        {
            if (ModelState.IsValid)
            {
                // 处理图片上传（如果有新图片）
                if (course.ImageFile != null && course.ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(course.ImageFile.FileName);
                    var uploadPath = Path.Combine(Server.MapPath("~/Uploads/CourseImages"), fileName);

                    // 删除旧图片（如果存在）
                    if (!string.IsNullOrEmpty(course.ImageUrl))
                    {
                        var oldFilePath = Server.MapPath("~" + course.ImageUrl);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    course.ImageFile.SaveAs(uploadPath);
                    course.ImageUrl = "/Uploads/CourseImages/" + fileName;
                }

                db.Entry(course).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);

            // 删除相关图片
            if (!string.IsNullOrEmpty(course.ImageUrl))
            {
                var filePath = Server.MapPath("~" + course.ImageUrl);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}