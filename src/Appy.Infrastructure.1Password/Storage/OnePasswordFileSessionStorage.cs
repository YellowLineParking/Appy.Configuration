using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Appy.Infrastructure.OnePassword.Model;
using static Appy.Infrastructure.OnePassword.Storage.KnownSessionVars;

namespace Appy.Infrastructure.OnePassword.Storage;

public class OnePasswordFileSessionStorage : IOnePasswordSessionStorage
{
    static readonly string UserProfileFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    static readonly string EnvFilePath =  Path.Combine(UserProfileFolderPath,  ".appy-op-env");
    static readonly char EnvValueSeparator = '=';
    static async Task<IEnumerable<string>> GetFileLines(string filePath)
    {
        var lines = new List<string>();
        using var sr = new StreamReader(filePath);
        var line = await sr.ReadLineAsync();
        while (!string.IsNullOrWhiteSpace(line))
        {
            lines.Add(line);
            line = await sr.ReadLineAsync();
        }

        return lines;
    }

    public async Task<AppyOnePasswordSession> GetCurrent()
    {
        if (!File.Exists(EnvFilePath))
        {
            return AppyOnePasswordSession.Empty();
        }

        var lines = await GetFileLines(EnvFilePath);

        var sessionLookup = lines
            .Select(line => line.Split(EnvValueSeparator))
            .ToDictionary(pair => pair[0], pair => pair[1].TrimEnd('\n', '\r'));

        var session = AppyOnePasswordSession.New(
            organization: sessionLookup.TryGetValue(OnePasswordOrganization, out var organization) ? organization : null,
            userId: sessionLookup.TryGetValue(OnePasswordUserId, out var userId) ? userId : null,
            environment: sessionLookup.TryGetValue(OnePasswordEnvironment, out var environment) ? environment : null,
            vault: sessionLookup.TryGetValue(KnownSessionVars.OnePasswordVault, out var vault) ? vault : null,
            sessionToken: sessionLookup.TryGetValue(OnePasswordSessionToken, out var sessionToken) ? sessionToken : null);

        return session;
    }

    public async Task Update(AppyOnePasswordSession session)
    {
        using var sw = new StreamWriter(EnvFilePath, false);
        await sw.WriteLineAsync($"{OnePasswordOrganization}{EnvValueSeparator}{session.Organization}");
        await sw.WriteLineAsync($"{OnePasswordUserId}{EnvValueSeparator}{session.UserId}");
        await sw.WriteLineAsync($"{OnePasswordEnvironment}{EnvValueSeparator}{session.Environment}");
        await sw.WriteLineAsync($"{KnownSessionVars.OnePasswordVault}{EnvValueSeparator}{session.Vault}");
        await sw.WriteLineAsync($"{KnownSessionVars.OnePasswordSessionToken}{EnvValueSeparator}{session.SessionToken}");
    }
}