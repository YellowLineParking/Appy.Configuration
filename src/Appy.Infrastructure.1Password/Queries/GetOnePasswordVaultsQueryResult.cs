using System.Collections.Generic;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Queries
{
    public class GetOnePasswordVaultsQueryResult
    {
        public IList<OnePasswordVault> Vaults { get; set; }
    }
}