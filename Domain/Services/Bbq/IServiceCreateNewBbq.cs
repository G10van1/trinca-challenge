using System.Threading.Tasks;
using Domain.Dtos;

namespace Domain.Services
{
    public interface IServiceCreateNewBbq
    {
        public Task<HttpResponse> CreateNewBbq(DtoNewBbqRequest input);
    }
}
