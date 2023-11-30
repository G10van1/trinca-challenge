using Domain.Events;
using Domain.Entities;
using Domain.Repositories;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Domain.Services
{
    internal class ServiceDeclineInvite : IServiceDeclineInvite
    {
        private readonly PersonId _user;
        private readonly IPersonRepository _people;
        private readonly IBbqRepository _bbqs;

        public ServiceDeclineInvite(PersonId user, IPersonRepository people, IBbqRepository bbqs)
        {
            _user = user;
            _people = people;
            _bbqs = bbqs;
        }
        public async Task<HttpResponse> DeclineInvite(string inviteId)
        {
            var person = await _people.GetAsync(_user.Id);

            if (person == null)
                return new HttpResponse(System.Net.HttpStatusCode.BadRequest, "Person not found");

            if (!person.Invites.Any(p => p.Id == inviteId))
                return new HttpResponse(System.Net.HttpStatusCode.BadRequest, "inviteId not found");

            try
            {
                person.Apply(new InviteWasDeclined { InviteId = inviteId, PersonId = person.Id });

                await _people.SaveAsync(person, null, person.Id);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                return new HttpResponse(System.Net.HttpStatusCode.InternalServerError, err.Message);
            }

            //Implementar impacto da recusa do convite no churrasco caso ele já tivesse sido aceito antes

            //Implementado para calcular através de uma lista de convidados do churras, onde a pessoa é 
            //inserida no momento do aceite ou rejeição do convite, sendo que a informação se é
            //vegetariano só é atualizada no caso de aceite.
            //A informação de status é atualizada para 'Accepted' ou 'Declined', conforme a solicitação.
            //Sendo assim para gerar a lista de compras é só calcular pela lista de convidados.
            try
            {
                var bbq = await _bbqs.GetAsync(inviteId);

                if (bbq == null)
                    return new HttpResponse(System.Net.HttpStatusCode.BadRequest, "inviteId not found");

                bbq.Apply(new InviteWasDeclined { InviteId = inviteId, PersonId = person.Id });

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
