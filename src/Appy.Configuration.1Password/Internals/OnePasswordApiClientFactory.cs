using System;
using System.Net.Http;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.ApiClient;

namespace Appy.Configuration.OnePassword.Internals
{
    public class OnePasswordApiClientFactory: IOnePasswordApiClientFactory
    {
        readonly IHttpClientFactory _httpClientFactory;
        readonly IAppyJsonSerializer _jsonSerializer;

        public OnePasswordApiClientFactory(
            IHttpClientFactory httpClientFactory,
            IAppyJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public OnePasswordApiClient Create()
        {
            var httpClient = _httpClientFactory.CreateClient(nameof(OnePasswordApiClient));

            return new OnePasswordApiClient(httpClient, _jsonSerializer);
        }
    }
}