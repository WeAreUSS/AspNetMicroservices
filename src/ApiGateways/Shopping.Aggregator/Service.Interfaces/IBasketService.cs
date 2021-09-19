using Shopping.Aggregator.Models;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Service.Interfaces
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);                
    }
}
