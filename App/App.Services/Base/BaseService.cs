using App.Services.Abstract;
using App.Services.Concrete;
using Microsoft.AspNetCore.Http;
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

        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseService(IHttpClientFactory httpClientFactory,IHttpContextAccessor httpContextAccessor)
        {
            DataClient = httpClientFactory.CreateClient("DataApi");
            FileClient = httpClientFactory.CreateClient("FileApi");
            _httpContextAccessor = httpContextAccessor;
        }

        protected async Task<HttpResponseMessage> SendAsync(HttpMethod method,string route,object? body = null,bool useFileClient = false)
        {
            var client = useFileClient ? FileClient : DataClient;

            var request = new HttpRequestMessage(method, route);

            var token = _httpContextAccessor.HttpContext?
                .User
                .FindFirst("access_token")?
                .Value;

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            if (body is not null)
            {
                if (body is HttpContent httpContent)
                {
                    request.Content = httpContent; 
                }
                else
                {
                    request.Content = JsonContent.Create(body); 
                }
            }

            return await client.SendAsync(request);
        }
    }
}
