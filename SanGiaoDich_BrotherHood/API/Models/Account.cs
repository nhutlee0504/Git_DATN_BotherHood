using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Account
    {
        [Key, Column(TypeName = "varchar(20)"), Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu"), Column(TypeName = "varchar(40)"), MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 kí tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên"), Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email"), EmailAddress(ErrorMessage = "Email không đúng định dạng"), Column(TypeName = "varchar(150)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại"), Column(TypeName = "varchar(12)")]
        [RegularExpression(@"^(0\d{9}|84\d{9})$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính"), Column(TypeName = "nvarchar(6)")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập CCCD"), Column(TypeName = "varchar(12)")]
        public string IDCard { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        public DateTime? Birthday { get; set; }

        [Required, Column(TypeName = "nvarchar(50)")]
        public string Role { get; set; }

        public string Introduce { get; set; }

        public string ImageAccount { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? TimeBanned { get; set; }

        public bool? IsDelete { get; set; }

        public ICollection<Product> products { get; set; }
        public ICollection<Cart> carts { get; set; }
        public ICollection<Favorite> favorites { get; set; }
        public ICollection<Rating> ratings { get; set; }
        public ICollection<AddressDetail> addressDetails {  get; set; } 
        public ICollection<Bill> bills { get; set; }
        public ICollection<Message> messageSend { get; set; }
        public ICollection<Message> messagesReceive { get; set; }
    }
}
