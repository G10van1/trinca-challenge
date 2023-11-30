using System.Net;
using Domain.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunGetProposedBbqs
    {
        private readonly IServiceGetProposedBbqs _service;
        public RunGetProposedBbqs(IServiceGetProposedBbqs service)
        {
            _service = service;
        }

        [Function(nameof(RunGetProposedBbqs))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras")] HttpRequestData req)
        {
            HttpResponse response;

            try
            {
                response = await _service.GetProposedBbqs();
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
