﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDProduct { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm"), Column(TypeName = "nvarchar(250)")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng sản phẩm")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        public decimal Price { get; set; }

        [ForeignKey("Category")]
        public int IDCategory { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; }

        [ForeignKey("Account"), Column(TypeName = "varchar(20)")]
        public string UserName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        public Account Account { get; set; }
        public ICollection<Cart> carts { get; set; }
        public ICollection<Favorite> favorites { get; set; }
        public ICollection<BillDetail> billDetails { get; set; }
        public ICollection<ImageProduct> imageProducts { get; set; }
        public Category Category { get; set; }
    }
}
