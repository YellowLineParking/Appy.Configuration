using System;
using System.Threading;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Storage;

namespace Appy.Infrastructure.OnePassword.Tooling;

public class OnePasswordToolConfigurationDecorator: IOnePasswordTool
{
    readonly IOnePasswordTool _innerTool;
    readonly IOnePasswordSessionStorage _sessionStorage;

    public OnePasswordToolConfigurationDecorator(
        IOnePasswordTool innerTool,
        IOnePasswordSessionStorage sessionStorage)
    {
        _innerTool = innerTool ?? throw new ArgumentNullException(nameof(innerTool));
        _sessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
    }

    public async Task<FetchOnePasswordNoteQueryResult> Execute(FetchOnePasswordNoteQuery query, CancellationToken cancellationToken = default)
    {
        var session = await _sessionStorage.GetCurrent();

        query.UserId = (query.UserId ?? session.UserId)!;
        query.Environment = (query.Environment ?? session.Environment)!;
        query.Vault = (query.Vault ?? session.Vault)!;
        query.SessionToken = (query.SessionToken ?? session.SessionToken)!;

        return await _innerTool.Execute(query, cancellationToken);
    }

    public Task<FetchOnePasswordVaultsQueryResult> Execute(FetchOnePasswordVaultsQuery query, CancellationToken cancellationToken = default)
    {
        return _innerTool.Execute(query, cancellationToken);
    }

    public Task<SignInOnePasswordResult> Execute(SignInOnePasswordCommand command, CancellationToken cancellationToken = default)
    {
        return _innerTool.Execute(command, cancellationToken);
    }
}