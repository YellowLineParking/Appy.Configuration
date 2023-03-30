using System.Threading;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Queries;

namespace Appy.Infrastructure.OnePassword.Tooling;

/// <summary>
/// 1Password wrapper tool around the 1Password CLI to get sections settings.
/// </summary>
public interface IOnePasswordTool
{
    /// <summary>
    /// Get 1Password Note environment section settings using the given organization, vault and session token.
    /// </summary>
    /// <param name="query">The query to get the 1Password note environment section settings</param>
    /// <param name="cancellationToken">The cancellationToken</param>
    /// <returns>The 1Password note environment section settings</returns>
    Task<GetOnePasswordNoteQueryResult> Execute(GetOnePasswordNoteQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get 1Password vaults using the given organization and session token
    /// </summary>
    /// <param name="query">The query to get the 1Password organization vaults</param>
    /// <param name="cancellationToken">The cancellationToken</param>
    /// <returns>The 1Password vaults for the organization</returns>
    Task<GetOnePasswordVaultsQueryResult> Execute(GetOnePasswordVaultsQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signin in 1Password and return a session token
    /// </summary>
    /// <param name="command">The query to get the 1Password note environment section settings</param>
    /// <param name="cancellationToken">The cancellationToken</param>
    /// <returns>A 1Password session token</returns>
    Task<SignInOnePasswordResult> Execute(SignInOnePasswordCommand command, CancellationToken cancellationToken = default);

}