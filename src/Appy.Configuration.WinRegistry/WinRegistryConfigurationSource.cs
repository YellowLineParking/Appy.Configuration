using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace Appy.Configuration.WinRegistry;

/// <summary>
/// Represents Windows Registry as and <see cref="IConfigurationSource"/>
/// </summary>
public class WinRegistryConfigurationSource : IConfigurationSource
{
    /// <summary>
    /// Creates <see cref="IConfigurationSource"/> for specified <paramref name="sectionPath"/> of Windows Registry
    /// </summary>
    /// <param name="registryRoot">registry root getter</param>
    /// <param name="sectionPath">section path</param>
    public WinRegistryConfigurationSource(
        Func<Microsoft.Win32.RegistryKey> registryRoot,
        string sectionPath)
    {
        if (string.IsNullOrWhiteSpace(sectionPath))
            throw new ArgumentException("Registry section name must be specified", nameof(sectionPath));

        SectionPath = sectionPath;

        RegistryRoot = registryRoot ?? throw new ArgumentNullException(nameof(registryRoot));
    }

    /// <summary> Gets Windows Registry section path
    /// </summary>
    public string SectionPath { get; }

    /// <summary> Gets Registry root getter
    /// </summary>
    public Func<RegistryKey> RegistryRoot { get; }

    /// <summary> Gets or sets optimal configuration data adapter.
    /// Will be invoked after reading data from registry.
    /// </summary>
    public Action<IDictionary<string, string?>>? DataAdapter { get; set; }

    /// <summary> Gets or sets configuration root section (optional).
    /// Specified value will be added as root section of registry keys.
    /// </summary>
    public string? RootSection { get; set; }

    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new WinRegistryConfigurationProvider(this);
}