using AspnetRunBasics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<UserInfoViewModel> GetUserInfo();

        Task<IEnumerable<CatalogModel>> GetCatalog();
        Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category);
        Task<CatalogModel> GetCatalog(string id);
        Task<CatalogModel> CreateCatalog(CatalogModel model);
    }
}
