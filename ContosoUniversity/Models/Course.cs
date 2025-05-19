using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ContosoUniversity.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }

        [Required(ErrorMessage = "课程标题是必填项")]
        [StringLength(50, ErrorMessage = "标题不能超过50个字符")]
        public string Title { get; set; }

        [Range(1, 5, ErrorMessage = "学分必须在1到5之间")]
        public int Credits { get; set; }

        // 新增：课程描述
        [Required(ErrorMessage = "课程描述是必填项")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        // 新增：课程图片URL（存储在数据库中）
        [Display(Name = "课程图片")]
        public string ImageUrl { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }

        // 非数据库字段，仅用于表单上传
        [NotMapped]
        [Display(Name = "上传图片")]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}