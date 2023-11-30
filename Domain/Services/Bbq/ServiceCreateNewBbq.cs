using Eveneum;
using System.Net;
using CrossCutting;
using Domain.Events;
using Domain.Entities;
using Domain.Repositories;
using System.Threading.Tasks;
using System;
using Domain.Dtos;

namespace Domain.Services
{    
    internal class ServiceCreateNewBbq : IServiceCreateNewBbq
    {
        private readonly PersonId _user;
        private readonly SnapshotStore _snapshots;
        private readonly IBbqRepository _bbqs;
        private readonly IPersonRepository _persons;
        public ServiceCreateNewBbq(IBbqRepository bbqs, IPersonRepository persons, SnapshotStore snapshots, PersonId user)
        {
            _user = user;
            _snapshots = snapshots;
            _bbqs = bbqs;
            _persons = persons;
        }
        public async Task<HttpResponse> CreateNewBbq(DtoNewBbqRequest input)
        {           
            var churras = new Bbq();
            churras.Apply(new ThereIsSomeoneElseInTheMood(Guid.NewGuid(), input.Date, input.Reason, input.IsTrincasPaying));

            await _bbqs.SaveAsync(churras, new { CreatedBy = _user.Id });

            var churrasSnapshot = churras.TakeSnapshot();

            var Lookups = await _snapshots.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();

            foreach (var personId in Lookups.ModeratorIds)
            {
                try
                {
                    var person = new Person();
                    person.Id = personId;
                    person.Apply(new PersonHasBeenInvitedToBbq(churras.Id, churras.Date, churras.Reason));

                    await _persons.SaveAsync(person, new { CreatedBy = _user.Id }, personId);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err);
                }
            }

            return new HttpResponse( HttpStatusCode.Created, churrasSnapshot );
        }
    }
}
