using Domain.Events;
using Domain.Entities;
using Domain.Repositories;
using System.Linq;
using System.Threading.Tasks;
using System;
using Domain.Dtos;

namespace Domain.Services
{
    internal class ServiceAcceptInvite : IServiceAcceptInvite
    {
        private readonly PersonId _user;
        private readonly IPersonRepository _people;
        private readonly IBbqRepository _bbqs;
        public ServiceAcceptInvite(IPersonRepository people, PersonId user, IBbqRepository bbqs)
        {
            _user = user;
            _people = people;
            _bbqs = bbqs;
        }
        public async Task<HttpResponse> AcceptInvite(DtoInviteAnswer answer, string inviteId)
        {
            var person = await _people.GetAsync(_user.Id);

            try
            {
                if (!person.Invites.Any(p => p.Id == inviteId))
                    return new HttpResponse(System.Net.HttpStatusCode.BadRequest, "inviteId not found");

                person.Apply(new InviteWasAccepted { InviteId = inviteId, IsVeg = answer.IsVeg, PersonId = person.Id });

                await _people.SaveAsync(person, null, person.Id);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                return new HttpResponse(System.Net.HttpStatusCode.InternalServerError, err.Message);
            }

            //implementar efeito do aceite do convite no churrasco
            //quando tiver 7 pessoas ele está confirmado
            try
            {
                var bbq = await _bbqs.GetAsync(inviteId);

                if (bbq == null)
                    return new HttpResponse(System.Net.HttpStatusCode.BadRequest, "inviteId not found");

                bbq.Apply(new InviteWasAccepted { InviteId = inviteId, IsVeg = answer.IsVeg, PersonId = person.Id });

                await _bbqs.SaveAsync(bbq, null, inviteId);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                return new HttpResponse(System.Net.HttpStatusCode.InternalServerError, err.Message);
            }

            return new HttpResponse(System.Net.HttpStatusCode.OK, person.TakeSnapshot());
        }
    }
}
