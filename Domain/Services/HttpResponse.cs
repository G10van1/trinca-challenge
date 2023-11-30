using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Domain.Services
{
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public object? Content { get; set; }
        public HttpResponse() { }
        public HttpResponse(HttpStatusCode statusCode, object? content = null) { 
            StatusCode = statusCode;
            Content = content;  
        }
    }
}
