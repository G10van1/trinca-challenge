using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IServiceGetProposedBbqs
    {
        public Task<HttpResponse> GetProposedBbqs();
    }
}
