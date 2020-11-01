using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Tooling
{
    public interface IOnePasswordSessionStorage
    {
        Task<AppyOnePasswordSession> GetCurrent();
        Task Update(AppyOnePasswordSession session);
    }
}