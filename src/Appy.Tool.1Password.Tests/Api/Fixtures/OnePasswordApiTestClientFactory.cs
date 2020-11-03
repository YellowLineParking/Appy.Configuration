using System.Net.Http;
using Appy.Infrastructure.OnePassword.ApiClient;

namespace Appy.Tool.OnePassword.Tests.Api.Fixtures
{
    public class OnePasswordApiTestClientFactory: IOnePasswordApiClientFactory
    {
        private readonly HttpClient _httpClient;

        public OnePasswordApiTestClientFactory(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public OnePasswordApiClient Create()
        {
            return new OnePasswordApiClient(_httpClient);
        }
    }
}