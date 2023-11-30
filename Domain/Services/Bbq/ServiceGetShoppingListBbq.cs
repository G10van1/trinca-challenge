using CrossCutting;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal class ServiceGetShoppingListBbq : IServiceGetShoppingListBbq
    {
        private readonly PersonId _user;
        private readonly SnapshotStore _snapshots;
        private readonly IPersonRepository _people;
        private readonly IBbqRepository _bbqs;
        private const int QTY_MEAT_NOVEG = 300;
        private const int QTY_VEGET_NOVEG = 300;
        private const int QTY_VEGET_VEG = 600;

        public ServiceGetShoppingListBbq(IBbqRepository bbqs, SnapshotStore snapshots, IPersonRepository people, PersonId user)
        {
            _user = user;
            _people = people;
            _snapshots = snapshots;
            _bbqs = bbqs;
        }
        public async Task<HttpResponse> GetShoppingListBbq(string id)
        {
            var lookups = await _snapshots.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();

            if (!lookups.ModeratorIds.Contains(_user.Id))
                return new HttpResponse(System.Net.HttpStatusCode.Unauthorized, "Only allowed for moderators");

            var bbq = await _bbqs.GetAsync(id);

            if (bbq == null)
                return new HttpResponse(System.Net.HttpStatusCode.BadRequest, "Id not found");

            bool isCanceled = bbq.Status == BbqStatus.ItsNotGonnaHappen;

            if (isCanceled)
                return new HttpResponse(System.Net.HttpStatusCode.OK, "It has already been rejected");

            bool isConfirmed = bbq.Status == BbqStatus.Confirmed;

            if (!isConfirmed)
                return new HttpResponse(System.Net.HttpStatusCode.OK, "Still not confirmed");

            double numPeople = bbq.Guests.Count(x => x.Status == InviteStatus.Accepted);
            double numVegs = bbq.Guests.Count(x => x.Status == InviteStatus.Accepted && x.IsVeg);

            var QtyKgMeat = ((numPeople - numVegs) * QTY_MEAT_NOVEG / 1000).ToString("F2");
            var QtyKgVegetables = (((numPeople - numVegs) * QTY_VEGET_NOVEG + (numVegs * QTY_VEGET_VEG)) / 1000).ToString("F2");

            return new HttpResponse(System.Net.HttpStatusCode.OK, new { QtyKgMeat, QtyKgVegetables });
        }
    }
}
