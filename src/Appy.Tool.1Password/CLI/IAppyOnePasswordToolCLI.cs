using System.Threading.Tasks;

namespace Appy.Tool.OnePassword.CLI
{
    public interface IAppyOnePasswordToolCLI
    {
        Task<int> ExecuteAsync(params string[] args);
    }
}