using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SanGiaoDich_BrotherHood.Server.Dto
{
    public class ProductDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set;}
        public List<IFormFile> Images { get; set; }
    }
}
