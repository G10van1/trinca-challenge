using Domain.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public class RunGetShoppingListBbq
    {
        private readonly IServiceGetShoppingListBbq _service;

        public RunGetShoppingListBbq(IServiceGetShoppingListBbq service)
        {
            _service = service;
        }

        [Function(nameof(RunGetShoppingListBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{id}/shopping")] HttpRequestData req, string id)
        {
            HttpResponse response;

            try
            {
                response = await _service.GetShoppingListBbq(id);
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
