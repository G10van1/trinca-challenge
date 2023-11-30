using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Services;
using System.Net;

namespace Serverless_Api
{
    public class RunDeclineInvite
    {
        private readonly IServiceDeclineInvite _service;

        public RunDeclineInvite(IServiceDeclineInvite service)
        {
            _service = service;
        }

        [Function(nameof(RunDeclineInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/decline")] HttpRequestData req, string inviteId)
        {
            HttpResponse response;

            try
            {
                response = await _service.DeclineInvite(inviteId);
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return await req.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }

            return await req.CreateResponse(response.StatusCode, response.Content);
        }
    }
}
