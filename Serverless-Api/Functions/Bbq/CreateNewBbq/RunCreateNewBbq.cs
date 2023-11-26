using Eveneum;
using System.Net;
using CrossCutting;
using Domain.Events;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunCreateNewBbq
    {
        private readonly Person _user;
        private readonly SnapshotStore _snapshots;
        private readonly IBbqRepository _bbqs;
        private readonly IPersonRepository _persons;
        public RunCreateNewBbq(IBbqRepository bbqs, IPersonRepository persons, SnapshotStore snapshots, Person user)
        {
            _user = user;
            _snapshots = snapshots;
            _bbqs = bbqs;
            _persons = persons;
        }

        [Function(nameof(RunCreateNewBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
        {
            var input = await req.Body<NewBbqRequest>();

            if (input == null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required.");
            }

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

            return await req.CreateResponse(HttpStatusCode.Created, churrasSnapshot);
        }
    }
}
