using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Queries;
using Microsoft.Extensions.Configuration;

namespace Appy.Configuration.OnePassword;

/// <summary>
/// Configuration provider for 1Password
/// </summary>
public class OnePasswordConfigurationProvider : ConfigurationProvider
{
    readonly OnePasswordConfigurationSource _source;

    /// <summary>
    /// Creates a configuration provider for 1Password.
    /// </summary>
    /// <param name="source">The source settings.</param>
    public OnePasswordConfigurationProvider(OnePasswordConfigurationSource source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public override void Load() => LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    async Task LoadAsync()
    {
        var appSettingsName = $"{_source.AppName}.AppSettings";

        var query = new GetOnePasswordNoteQuery
        {
            Organization = _source.Organization,
            Item = appSettingsName,
            Vault = _source.Vault,
            Environment = _source.Environment,
            SessionToken = _source.SessionToken
        };

        var result = await _source.Tool.Execute(query);

        var data = new Dictionary<string, string>();

        if (result?.Fields == null)
        {
            throw new OnePasswordConfigurationException($"1Password {query.Environment} environment section fields cannot be empty");
        }

        foreach (var field in result.Fields)
        {
            if (string.IsNullOrWhiteSpace(field.Name))
            {
                throw new OnePasswordConfigurationException($"1Password {query.Environment} environment section fields name cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(field.Value))
            {
                throw new OnePasswordConfigurationException($"1Password {query.Environment} environment section field '{field.Name}' value cannot be empty");
            }

            data[field.Name!] = field.Value!;
        }

        Data = data!;
    }
}