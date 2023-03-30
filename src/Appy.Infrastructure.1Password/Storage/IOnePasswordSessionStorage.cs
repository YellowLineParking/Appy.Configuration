using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Storage;

public interface IOnePasswordSessionStorage
{
    Task<AppyOnePasswordSession> GetCurrent();
    Task Update(AppyOnePasswordSession session);
}