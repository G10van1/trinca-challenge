using Eveneum;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Dtos;
using Domain.Services;

namespace Serverless_Api
{
    public partial class RunCreateNewBbq
    {
        private readonly IServiceCreateNewBbq _service;
        public RunCreateNewBbq(IServiceCreateNewBbq service)
        {
            _service = service;
        }

        [Function(nameof(RunCreateNewBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
        {
            DtoNewBbqRequest? input = await req.Body<DtoNewBbqRequest>();

            if (input == null)
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required");

            HttpResponse response;

            try
            {
                response = await _service.CreateNewBbq(input);
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
