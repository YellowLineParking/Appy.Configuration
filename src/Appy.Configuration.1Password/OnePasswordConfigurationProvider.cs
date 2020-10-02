using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Tooling;
using Microsoft.Extensions.Configuration;

namespace Appy.Configuration.OnePassword
{
    /// <summary>
    /// Configuration provider for 1Password
    /// </summary>
    public class OnePasswordConfigurationProvider : ConfigurationProvider
    {
        readonly OnePasswordConfigurationSource _source;

        /// <summary>
        /// Creates configuration provider for 1Password.
        /// </summary>
        /// <param name="source">The source settings.</param>
        public OnePasswordConfigurationProvider(OnePasswordConfigurationSource source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public override void Load() => LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        async Task LoadAsync()
        {
            var query = new OnePasswordGetNoteQuery
            {
                Organisation = _source.Organization,
                Item = _source.AppSettingsName,
                Vault = _source.Vault,
                Environment = _source.Environment,
                SessionToken = _source.SessionToken
            };

            var result = await _source.Tool.Execute(query);

            var data = new Dictionary<string, string>();

            var sectionFields = result.EnvironmentSection?.Fields;
            if (sectionFields == null)
            {
                throw new OnePasswordToolException("1Password environment section settings cannot be null");
            }

            foreach (var sectionField in sectionFields)
            {
                if (string.IsNullOrWhiteSpace(sectionField.Name))
                {
                    throw new OnePasswordToolException("1Password Section fields name cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(sectionField.Value))
                {
                    throw new OnePasswordToolException($"1Password Section field '{sectionField.Name} value cannot be empty");
                }

                data[sectionField.Name!] = sectionField.Value!;
            }

            Data = data;
        }
    }
}