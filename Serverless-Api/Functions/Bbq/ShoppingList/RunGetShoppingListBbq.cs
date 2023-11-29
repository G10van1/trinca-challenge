using CrossCutting;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunGetShoppingListBbq
    {
        private readonly Person _user;
        private readonly SnapshotStore _snapshots;
        private readonly IPersonRepository _persons;
        private readonly IBbqRepository _bbqs;
        private const int QTY_MEAT_NOVEG = 300;
        private const int QTY_VEGET_NOVEG = 300;
        private const int QTY_VEGET_VEG = 600;

        public RunGetShoppingListBbq(IBbqRepository bbqs, SnapshotStore snapshots, IPersonRepository persons, Person user)
        {
            _user = user;
            _persons = persons;
            _snapshots = snapshots;
            _bbqs = bbqs;
        }

        [Function(nameof(RunGetShoppingListBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{id}/shopping")] HttpRequestData req, string id)
        {
            var lookups = await _snapshots.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();
            
            if (!lookups.ModeratorIds.Contains(_user.Id))
                return await req.CreateResponse(System.Net.HttpStatusCode.Unauthorized, "Only allowed for moderators");
                        
            var bbq = await _bbqs.GetAsync(id);

            if (bbq == null)
                return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Id not found");

            bool isCanceled = bbq.Status == BbqStatus.ItsNotGonnaHappen;

            if (isCanceled)
                return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "It has already been rejected");

            bool isConfirmed = bbq.Status == BbqStatus.Confirmed;
            
            if (!isConfirmed)
                return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Still not confirmed");

            double numPeople = bbq.Guests.Count(x => x.Status == InviteStatus.Accepted);
            double numVegs = bbq.Guests.Count(x => x.Status == InviteStatus.Accepted && x.IsVeg);

            var QtyKgMeat = ((numPeople - numVegs) * QTY_MEAT_NOVEG / 1000).ToString("F2");
            var QtyKgVegetables = (((numPeople - numVegs) * QTY_VEGET_NOVEG + (numVegs * QTY_VEGET_VEG)) / 1000).ToString("F2");

            return await req.CreateResponse(System.Net.HttpStatusCode.OK, new {QtyKgMeat, QtyKgVegetables});
        }
    }
}
