using System.Collections.Generic;
using System.Threading.Tasks;

using Catalog.API.Entities;

namespace Catalog.API.Repositories.Interfaces
{
    public interface IProductRepository
    {
        #region Get Operations
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProduct(string id);
        Task<IEnumerable<Product>> GetProductByName(string name);
        Task<IEnumerable<Product>> GetProductByCategory(string categoryName);
        #endregion Get Operations

        #region CRUD Operations
        Task CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(string id);
        #endregion CRUD Operations
    }
}