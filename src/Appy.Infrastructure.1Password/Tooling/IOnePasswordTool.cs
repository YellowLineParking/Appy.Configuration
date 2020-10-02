using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Queries;

namespace Appy.Infrastructure.OnePassword.Tooling
{
    /// <summary>
    /// 1Password wrapper tool around the 1Password CLI to get sections settings.
    /// </summary>
    public interface IOnePasswordTool
    {
        /// <summary>
        /// Get 1Password Note environment section settings using the given organization, vault and session token.
        /// </summary>
        /// <param name="query">The query to get the 1Password note environment section settings</param>
        /// <returns>The 1Password note environment section settings</returns>
        Task<OnePasswordGetNoteQueryResult> Execute(OnePasswordGetNoteQuery query);
    }
}