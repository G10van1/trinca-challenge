using Domain.Dtos;
using Domain.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public class RunModerateBbq
    {
        private readonly IServiceModerateBbq _service;

        public RunModerateBbq(IServiceModerateBbq service)
        {
            _service = service;
        }

        [Function(nameof(RunModerateBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{id}/moderar")] HttpRequestData req, string id)
        {
            DtoModerateBbqRequest moderationRequest;
            try
            {
                moderationRequest = await req.Body<DtoModerateBbqRequest>();
            }
            catch (Exception err)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required");
            }

            HttpResponse response;

            try
            {
                response = await _service.ModerateBbq(moderationRequest, id);
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
