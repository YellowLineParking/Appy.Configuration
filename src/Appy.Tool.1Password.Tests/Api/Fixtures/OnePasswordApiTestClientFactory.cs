using System.Net.Http;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.ApiClient;

namespace Appy.Tool.OnePassword.Tests.Api.Fixtures;

public class OnePasswordApiTestClientFactory: IOnePasswordApiClientFactory
{
    readonly HttpClient _httpClient;
    readonly IAppyJsonSerializer _jsonSerializer;

    public OnePasswordApiTestClientFactory(HttpClient httpClient, IAppyJsonSerializer jsonSerializer)
    {
        _httpClient = httpClient;
        _jsonSerializer = jsonSerializer;
    }

    public OnePasswordApiClient Create()
    {
        return new OnePasswordApiClient(_httpClient, _jsonSerializer);
    }
}