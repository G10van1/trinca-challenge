using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories;

namespace Domain.Services
{
    internal class ServiceGetProposedBbqs : IServiceGetProposedBbqs
    {
        private readonly PersonId _user;
        private readonly IBbqRepository _bbqs;
        private readonly IPersonRepository _persons;
        public ServiceGetProposedBbqs(IPersonRepository persons, IBbqRepository bbqs, PersonId user)
        {
            _user = user;
            _bbqs = bbqs;
            _persons = persons;
        }
        public async Task<HttpResponse> GetProposedBbqs()
        {
            var snapshots = new List<object>();
            var user = await _persons.GetAsync(_user.Id);

            if (user == null)
                return new HttpResponse(System.Net.HttpStatusCode.NoContent);

            foreach (var bbqId in user.Invites.Where(i => i.Date > DateTime.Now).Select(o => o.Id).ToList())
            {
                var bbq = await _bbqs.GetAsync(bbqId);

                if (bbq.Status != BbqStatus.ItsNotGonnaHappen)
                    snapshots.Add(bbq.TakeSnapshot());
            }

            return new HttpResponse(HttpStatusCode.Created, snapshots);
        }
    }
}
