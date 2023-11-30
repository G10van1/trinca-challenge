using Domain.Dtos;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IServiceAcceptInvite
    {
        public Task<HttpResponse> AcceptInvite(DtoInviteAnswer answer, string inviteId);
    }
}
