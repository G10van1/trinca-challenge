using CrossCutting;
using Domain.Dtos;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal class ServiceModerateBbq : IServiceModerateBbq
    {
        private readonly PersonId _user;
        private readonly SnapshotStore _snapshots;
        private readonly IPersonRepository _persons;
        private readonly IBbqRepository _bbqs;

        public ServiceModerateBbq(IBbqRepository bbqs, SnapshotStore snapshots, IPersonRepository persons, PersonId user)
        {
            _user = user;
            _persons = persons;
            _snapshots = snapshots;
            _bbqs = bbqs;
        }

        public async Task<HttpResponse> ModerateBbq(DtoModerateBbqRequest moderationRequest, string id)
        {
            var lookups = await _snapshots.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();

            if (!lookups.ModeratorIds.Contains(_user.Id))
                return new HttpResponse(System.Net.HttpStatusCode.Unauthorized, "Only allowed for moderators");

            if (!moderationRequest.GonnaHappen && moderationRequest.TrincaWillPay)
                return new HttpResponse(System.Net.HttpStatusCode.BadRequest, "If GonnaHappen is false, TrincaWillPay must be false");

            var bbq = await _bbqs.GetAsync(id);

            if (bbq == null)
                return new HttpResponse(System.Net.HttpStatusCode.BadRequest, "Id not found");

            bool isCanceled = bbq.Status == BbqStatus.ItsNotGonnaHappen;

            if (isCanceled)
                return new HttpResponse(System.Net.HttpStatusCode.BadRequest, "No changes allowed, it has already been rejected");

            bool isNew = bbq.Status == BbqStatus.New;

            if (!isNew && moderationRequest.GonnaHappen)
                return new HttpResponse(System.Net.HttpStatusCode.BadRequest, "It has already been approved");

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
                        return new HttpResponse(System.Net.HttpStatusCode.InternalServerError, err.Message);
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
                        return new HttpResponse(System.Net.HttpStatusCode.InternalServerError, err.Message);
                    }
                }

            await _bbqs.SaveAsync(bbq);

            return new HttpResponse(System.Net.HttpStatusCode.OK, bbq.TakeSnapshot());
        }
    }
}

