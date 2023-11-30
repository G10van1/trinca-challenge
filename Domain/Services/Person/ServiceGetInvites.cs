using Domain.Entities;
using Domain.Repositories;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal class ServiceGetInvites : IServiceGetInvites
    {
        private readonly PersonId _user;
        private readonly IPersonRepository _people;

        public ServiceGetInvites(PersonId user, IPersonRepository people)
        {
            _user = user;
            _people = people;
        }
        public async Task<HttpResponse> GetInvites()
        {
            var person = await _people.GetAsync(_user.Id);

            if (person == null)
                return new HttpResponse(System.Net.HttpStatusCode.NoContent);

            return new HttpResponse(System.Net.HttpStatusCode.OK, person.TakeSnapshot());
        }
    }
}
