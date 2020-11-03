using System.Threading.Tasks;

namespace Appy.Tool.OnePassword.Cli
{
    public interface IAppyOnePasswordToolCli
    {
        Task<int> ExecuteAsync(params string[] args);
    }
}