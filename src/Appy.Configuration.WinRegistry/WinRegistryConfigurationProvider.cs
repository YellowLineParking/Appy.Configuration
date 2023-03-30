using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace Appy.Configuration.WinRegistry;

/// <summary>
/// Configuration provider for Windows Registry
/// </summary>
public class WinRegistryConfigurationProvider : ConfigurationProvider
{
    readonly WinRegistryConfigurationSource _source;

    /// <summary> Creates configuration provider for Windows Registry.
    /// </summary>
    /// <param name="source">The source settings.</param>
    public WinRegistryConfigurationProvider(WinRegistryConfigurationSource source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

#pragma warning disable CA1416

    /// <inheritdoc />
    public override void Load()
    {
        var root = _source.RegistryRoot.Invoke();

        if (root == null)
        {
            throw new ArgumentOutOfRangeException(nameof(_source), $"{nameof(WinRegistryConfigurationSource.RegistryRoot)} must return not null instance of {_source.RegistryRoot.Method.ReturnType.Name}");
        }

        var section = root.OpenSubKey(_source.SectionPath);

        if (section == null)
        {
            Data = new Dictionary<string, string>();
        }

        var data = new Dictionary<string, string?>();
        var prefixStack = new Stack<string?>();

        if (!string.IsNullOrWhiteSpace(_source.RootSection))
        {
            prefixStack.Push(_source.RootSection);
        }

        try
        {
            ReadSection(section, data, prefixStack);
        }
        finally
        {
            section?.Dispose();
        }

        _source.DataAdapter?.Invoke(data);

        Data = data;

    }

    static void ReadSection(RegistryKey? section, Dictionary<string, string?> data, Stack<string?> prefixStack)
    {
        foreach (var subKeyName in section?.GetSubKeyNames()!)
        {
            prefixStack.Push(subKeyName);

            using (var subKey = section.OpenSubKey(subKeyName))
            {
                ReadSection(subKey, data, prefixStack);
            }

            prefixStack.Pop();
        }

        foreach (var valueName in section.GetValueNames())
        {
            prefixStack.Push(valueName);

            data[ConfigurationPath.Combine(prefixStack.Reverse())] = section.GetValue(valueName)?.ToString();

            prefixStack.Pop();
        }
    }

#pragma warning restore CA1416

}