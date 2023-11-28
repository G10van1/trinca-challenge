using Domain;
using Eveneum;
using CrossCutting;
using Domain.Events;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;


namespace Serverless_Api
{
    public partial class RunDeclineInvite
    {
        private readonly Person _user;
        private readonly IPersonRepository _repository;
        private readonly IBbqRepository _bbqRepository;

        public RunDeclineInvite(Person user, IPersonRepository repository, IBbqRepository bbqRepository)
        {
            _user = user;
            _repository = repository;
            _bbqRepository = bbqRepository;
        }

        [Function(nameof(RunDeclineInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/decline")] HttpRequestData req, string inviteId)
        {
            var person = await _repository.GetAsync(_user.Id);

            if (person == null)
                return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Person not found");

            if (!person.Invites.Any(p => p.Id == inviteId))
                return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "inviteId not found");
            
            try
            {
                person.Apply(new InviteWasDeclined { InviteId = inviteId, PersonId = person.Id });

                await _repository.SaveAsync(person, null, person.Id);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                return await req.CreateResponse(System.Net.HttpStatusCode.InternalServerError, err.Message);
            }

            //Implementar impacto da recusa do convite no churrasco caso ele já tivesse sido aceito antes
            try
            {
                var bbq = await _bbqRepository.GetAsync(inviteId);

                if (bbq == null)
                    return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "inviteId not found");

                bbq.Apply(new InviteWasDeclined { InviteId = inviteId, PersonId = person.Id });

                await _bbqRepository.SaveAsync(bbq, null, inviteId);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                return await req.CreateResponse(System.Net.HttpStatusCode.InternalServerError, err.Message);
            }

            return await req.CreateResponse(System.Net.HttpStatusCode.OK, person.TakeSnapshot());
        }
    }
}
