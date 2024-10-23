using SanGiaoDich_BrotherHood.Server.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IProduct
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsAccount();
        Task<IEnumerable<Product>> GetProductByNameAccount(string username);
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> GetProductByName(string name);
        Task<Product> AddProduct(ProductDto product);
        Task<Product> UpdateProductById(int id, ProductDto product);
        Task<Product> DeleteProductById(int id);
    }
}
