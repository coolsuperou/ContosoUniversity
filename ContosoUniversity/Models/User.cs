using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ContosoUniversity.Models
{
    [Table("tb_user")]
    public partial class User
    {
        [Key]
        [DisplayName("用户账号")]
        public int uid { get; set; }

        [DisplayName("用户名称")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户名称不能为空")]
        public string name { get; set; }

        [DisplayName("用户密码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户密码不能为空")]
        [DataType(DataType.Password, ErrorMessage = "密码格式输入错误")]
        public string password { get; set; }

        [Compare("password")]
        [Required]
        [DisplayName("确认密码")]
        [DataType(DataType.Password)]
        [NotMapped]
        public string confirmPassword { get; set; }

        [DisplayName("用户地址")]
        public string address { get; set; }

        [DisplayName("用户电话")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "电话号码格式不正确")]
        public string tel { get; set; }

        [DisplayName("用户邮箱")]
        [DataType(DataType.EmailAddress, ErrorMessage = "邮箱格式不正确")]
        public string email { get; set; }
    }
}
