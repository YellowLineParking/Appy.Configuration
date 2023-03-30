namespace Appy.Configuration.Common;

public interface IEnvironmentAccessor
{
    string GetUserEnvironmentVariable(string environmentVariable);
    string GetProcessEnvironmentVariable(string environmentVariable);
    void SetUserEnvironmentVariable(string environmentVariable, string? value);
}