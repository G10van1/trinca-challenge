using CrossCutting;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunModerateBbq
    {
        private readonly Person _user;
        private readonly SnapshotStore _snapshots;
        private readonly IPersonRepository _persons;
        private readonly IBbqRepository _repository;

        public RunModerateBbq(IBbqRepository repository, SnapshotStore snapshots, IPersonRepository persons, Person user)
        {
            _user = user;
            _persons = persons;
            _snapshots = snapshots;
            _repository = repository;
        }

        [Function(nameof(RunModerateBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{id}/moderar")] HttpRequestData req, string id)
        {
            var lookups = await _snapshots.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();
            
            if (!lookups.ModeratorIds.Contains(_user.Id))
                return await req.CreateResponse(System.Net.HttpStatusCode.Unauthorized, "Only allowed for moderators");

            var moderationRequest = await req.Body<ModerateBbqRequest>();

            if (!moderationRequest.GonnaHappen && moderationRequest.TrincaWillPay)
                return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "If GonnaHappen is false, TrincaWillPay must be false");

            var bbq = await _repository.GetAsync(id);

            if (bbq == null)
                return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Id not found");

            bool isCanceled = bbq.Status == BbqStatus.ItsNotGonnaHappen;
            
            if (isCanceled)
                return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "No changes allowed, it has already been rejected");

            bool isNew = bbq.Status == BbqStatus.New;

            if (!isNew && moderationRequest.GonnaHappen)
                return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "It has already been approved");

            bbq.Apply(new BbqStatusUpdated(moderationRequest.GonnaHappen, moderationRequest.TrincaWillPay));

            if (moderationRequest.GonnaHappen)
                foreach (var personId in lookups.PeopleIds)
                {
                    try
                    {
                        if (lookups.ModeratorIds.Contains(personId))
                            continue;
                        var person = await _persons.GetAsync(personId);
                        var @event = new PersonHasBeenInvitedToBbq(bbq.Id, bbq.Date, bbq.Reason);
                        person.Apply(@event);
                        await _persons.SaveAsync(person, null, personId);
                    }
                    catch (Exception err)
                    {
                        Console.Write(err);
                        return await req.CreateResponse(System.Net.HttpStatusCode.InternalServerError, err.Message);
                    }
                }
            else
                foreach (var personId in lookups.PeopleIds)
                {
                    try
                    {
                        var person = await _persons.GetAsync(personId);
                        if (person.Invites.Any(x => x.Id == id))
                        {
                            var @event = new InviteWasDeclined(id, personId);
                            person.Apply(@event);
                            await _persons.SaveAsync(person, null, personId);
                        }
                    }
                    catch (Exception err)
                    {
                        Console.Write(err);
                        return await req.CreateResponse(System.Net.HttpStatusCode.InternalServerError, err.Message);
                    }
                }

            await _repository.SaveAsync(bbq);

            return await req.CreateResponse(System.Net.HttpStatusCode.OK, bbq.TakeSnapshot());
        }
    }
}
