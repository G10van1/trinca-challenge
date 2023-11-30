using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Services;
using Domain.Dtos;
using System.Net;

namespace Serverless_Api
{
    public partial class RunAcceptInvite
    {
        private readonly IServiceAcceptInvite _service;
        public RunAcceptInvite(IServiceAcceptInvite service)
        {
            _service = service;
        }

        [Function(nameof(RunAcceptInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/accept")] HttpRequestData req, string inviteId)
        {
            var answer = await req.Body<DtoInviteAnswer>();

            HttpResponse response;

            try
            {
                response = await _service.AcceptInvite(answer, inviteId);
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
