using Domain.Events;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunAcceptInvite
    {
        private readonly Person _user;
        private readonly IPersonRepository _repository;
        private readonly IBbqRepository _bbqRepository;
        public RunAcceptInvite(IPersonRepository repository, Person user, IBbqRepository bbqsRepository)
        {
            _user = user;
            _repository = repository;
            _bbqRepository = bbqsRepository;
        }

        [Function(nameof(RunAcceptInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/accept")] HttpRequestData req, string inviteId)
        {
            var answer = await req.Body<InviteAnswer>();

            var person = await _repository.GetAsync(_user.Id);

            try
            {   
                if (!person.Invites.Any(p => p.Id == inviteId))
                    return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "inviteId not found");

                person.Apply(new InviteWasAccepted { InviteId = inviteId, IsVeg = answer.IsVeg, PersonId = person.Id });

                await _repository.SaveAsync(person, null, person.Id);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                return await req.CreateResponse(System.Net.HttpStatusCode.InternalServerError, err.Message);
            }

            //implementar efeito do aceite do convite no churrasco
            //quando tiver 7 pessoas ele está confirmado
            try
            {
                var bbq = await _bbqRepository.GetAsync(inviteId);

                if (bbq == null)
                    return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "inviteId not found");

                bbq.Apply(new InviteWasAccepted { InviteId = inviteId, IsVeg = answer.IsVeg, PersonId = person.Id });

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
