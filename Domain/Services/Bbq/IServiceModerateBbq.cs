using Eveneum;
using System.Net;
using CrossCutting;
using System.Threading.Tasks;
using Domain.Dtos;

namespace Domain.Services
{
    public interface IServiceModerateBbq
    {
        public Task<HttpResponse> ModerateBbq(DtoModerateBbqRequest input, string id);
    }
}
