using System;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Storage;

public class OnePasswordSessionEmptyStorage : IOnePasswordSessionStorage
{
    public Task<AppyOnePasswordSession> GetCurrent()
    {
        return Task.FromResult(AppyOnePasswordSession.Empty());
    }

    public Task Update(AppyOnePasswordSession session)
    {
        throw new NotImplementedException("Using Remote API");
    }
}