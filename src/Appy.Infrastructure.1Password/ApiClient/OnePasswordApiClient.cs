using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Queries;

namespace Appy.Infrastructure.OnePassword.ApiClient
{
    public class OnePasswordApiClient
    {
        readonly HttpClient _httpClient;
        readonly IAppyJsonSerializer _jsonSerializer;

        public OnePasswordApiClient(
            HttpClient httpClient,
            IAppyJsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
        }

        public virtual Task<Response<GetOnePasswordNoteQueryResult>> Execute(GetOnePasswordNoteQuery query, CancellationToken cancellationToken = default)
        {
            return PostJsonAsync<GetOnePasswordNoteQueryResult>("queries/getOnePasswordNote", query, cancellationToken);
        }

        async Task<Response<TResponse>> PostJsonAsync<TResponse>(string url, object request, CancellationToken cancellationToken)
        {
            var content = _jsonSerializer.Serialize(request);

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_httpClient.BaseAddress, url),
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };

            var rawResponse = await _httpClient.SendAsync(requestMessage, cancellationToken);

            var json = await rawResponse.Content.ReadAsStringAsync();

            var response = _jsonSerializer.Deserialize<Response<TResponse>>(json);

            if (rawResponse.IsSuccessStatusCode)
                return response;

            throw new OnePasswordApiClientException(
                statusCode: (int)rawResponse.StatusCode,
                response);
        }
    }
}