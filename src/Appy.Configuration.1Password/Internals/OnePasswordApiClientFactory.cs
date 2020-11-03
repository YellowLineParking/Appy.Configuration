using System;
using System.Net.Http;
using Appy.Infrastructure.OnePassword.ApiClient;

namespace Appy.Configuration.OnePassword.Internals
{
    public class OnePasswordApiClientFactory: IOnePasswordApiClientFactory
    {
        readonly IHttpClientFactory _httpClientFactory;

        public OnePasswordApiClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public OnePasswordApiClient Create()
        {
            var httpClient = _httpClientFactory.CreateClient(nameof(OnePasswordApiClient));
            
            return new OnePasswordApiClient(httpClient);
        }
    }
}