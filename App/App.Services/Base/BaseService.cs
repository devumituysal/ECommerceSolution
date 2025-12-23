using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Base
{
    public abstract class BaseService
    {
        protected HttpClient DataClient { get; }
        protected HttpClient FileClient { get; }

        protected BaseService(IHttpClientFactory httpClientFactory)
        {
            DataClient = httpClientFactory.CreateClient("DataApi");
            FileClient = httpClientFactory.CreateClient("FileApi");
        }

        protected async Task<HttpResponseMessage> SendAsync(HttpMethod method,string route,string jwt,object? body = null)
        {
            var request = new HttpRequestMessage(method, route);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);

            if (body is not null)
            {
                request.Content = JsonContent.Create(body);
            }

            return await DataClient.SendAsync(request);
        }
    }
}
