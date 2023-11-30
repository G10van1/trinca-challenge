using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IServiceGetShoppingListBbq
    {
        public Task<HttpResponse> GetShoppingListBbq(string id);
    }
}
