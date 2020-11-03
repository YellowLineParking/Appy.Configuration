using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Queries;
using Flurl.Http;

namespace Appy.Infrastructure.OnePassword.ApiClient
{
    public class OnePasswordApiClient
    {
        readonly FlurlClient  _client;

        public OnePasswordApiClient(HttpClient httpClient)
        {
            _client = new FlurlClient(httpClient);
        }

        public virtual Task<Response<GetOnePasswordNoteQueryResult>> Execute(GetOnePasswordNoteQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _client
                .Request("queries/getOnePasswordNote")
                .PostJsonAsync(query, cancellationToken)
                .ReceiveJson<Response<GetOnePasswordNoteQueryResult>>();
        }
    }
}