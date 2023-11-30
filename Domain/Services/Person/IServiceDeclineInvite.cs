using Domain.Dtos;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IServiceDeclineInvite
    {
        public Task<HttpResponse> DeclineInvite(string inviteId);
    }
}
