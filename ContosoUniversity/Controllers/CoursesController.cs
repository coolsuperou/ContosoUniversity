using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

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

        // GET: PrintCourses
        public ActionResult PrintCourses()
        {
            var courses = db.Courses.Include("Enrollments").ToList();

            // 创建PDF文档
            Document document = new Document(PageSize.A4.Rotate(), 20, 20, 30, 30); // 横向页面，边距调整
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            // 添加标题
            Paragraph title = new Paragraph("课程列表 - 打印日期: " + DateTime.Now.ToString("yyyy-MM-dd"),
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18));
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            document.Add(new Paragraph("\n"));

            // 创建表格 - 现在有5列（新增图片列）
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1, 2, 1, 3, 2 }); // 调整列宽比例

            // 添加表头
            AddCell(table, "课程ID", true);
            AddCell(table, "课程名称", true);
            AddCell(table, "学分", true);
            AddCell(table, "描述", true);
            AddCell(table, "课程图片", true);

            // 添加课程数据
            foreach (var course in courses)
            {
                AddCell(table, course.CourseID.ToString());
                AddCell(table, course.Title);
                AddCell(table, course.Credits.ToString());
                AddCell(table, WebUtility.HtmlDecode(Regex.Replace(course.Description, "<.*?>", string.Empty)));

                // 添加图片单元格
                PdfPCell imageCell = new PdfPCell();
                imageCell.Padding = 5;
                imageCell.HorizontalAlignment = Element.ALIGN_CENTER;

                if (!string.IsNullOrEmpty(course.ImageUrl))
                {
                    try
                    {
                        string imagePath = Server.MapPath("~" + course.ImageUrl);
                        if (System.IO.File.Exists(imagePath))
                        {
                            Image image = Image.GetInstance(imagePath);
                            image.ScaleToFit(80f, 60f); // 调整图片大小
                            imageCell.AddElement(image);
                        }
                        else
                        {
                            imageCell.AddElement(new Phrase("图片未找到", FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                        }
                    }
                    catch
                    {
                        imageCell.AddElement(new Phrase("图片加载错误", FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                    }
                }
                else
                {
                    imageCell.AddElement(new Phrase("无图片", FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                }

                table.AddCell(imageCell);
            }

            document.Add(table);

            // 添加页脚
            Paragraph footer = new Paragraph($"共 {courses.Count} 门课程",
                FontFactory.GetFont(FontFactory.HELVETICA, 10));
            footer.Alignment = Element.ALIGN_RIGHT;
            document.Add(footer);

            document.Close();

            // 返回PDF文件
            return File(memoryStream.ToArray(), "application/pdf", "课程列表_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
        }

        // 辅助方法：添加表格单元格（文本）
        private void AddCell(PdfPTable table, string text, bool isHeader = false)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5;

            if (isHeader)
            {
                cell.BackgroundColor = new BaseColor(200, 200, 200);
                cell.Phrase.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            }
            else
            {
                cell.Phrase.Font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            }

            table.AddCell(cell);
        }

        // 其他方法保持不变...
        // GET: Courses/Details/5
        public ActionResult Details(int? id)
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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