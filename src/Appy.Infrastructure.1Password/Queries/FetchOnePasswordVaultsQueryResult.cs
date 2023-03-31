using System.Collections.Generic;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Queries;

public class FetchOnePasswordVaultsQueryResult
{
    public IReadOnlyCollection<OnePasswordVault> Vaults { get; set; }
}