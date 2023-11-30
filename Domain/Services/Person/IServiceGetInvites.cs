using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IServiceGetInvites
    {
        public Task<HttpResponse> GetInvites();
    }
}