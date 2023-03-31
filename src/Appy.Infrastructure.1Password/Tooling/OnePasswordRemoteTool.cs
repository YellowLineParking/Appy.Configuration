using System;
using System.Threading;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.ApiClient;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Queries;

namespace Appy.Infrastructure.OnePassword.Tooling;

public class OnePasswordRemoteTool: IOnePasswordTool
{
    readonly IOnePasswordApiClientFactory  _apiClientFactory;

    public OnePasswordRemoteTool(IOnePasswordApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
    }

    public Task<FetchOnePasswordNoteQueryResult> Execute(FetchOnePasswordNoteQuery query, CancellationToken cancellationToken = default(CancellationToken))
    {
        return _apiClientFactory.Create().Execute(query, cancellationToken).UnWrap();
    }

    public Task<FetchOnePasswordVaultsQueryResult> Execute(FetchOnePasswordVaultsQuery query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Available only in the local version.");
    }

    public Task<SignInOnePasswordResult> Execute(SignInOnePasswordCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Available only in the local version.");
    }
}