﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Favorite
    {
        [Key]
        public int IDFavorite { get; set; }

        [ForeignKey("Account"), Column(TypeName = "varchar(20)")]
        public string UserName { get; set; }

        [ForeignKey("Product")]
        public int IDProduct {  get; set; }

        public Account Account { get; set; }
        public Product Product { get; set; }
    }
}
