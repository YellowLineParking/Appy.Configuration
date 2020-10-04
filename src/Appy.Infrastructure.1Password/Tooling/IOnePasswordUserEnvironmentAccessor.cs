namespace Appy.Infrastructure.OnePassword.Tooling
{
    public interface IOnePasswordUserEnvironmentAccessor
    {
        string GetOrganization();
        string GetExecutionEnvironment();
        string GetSessionToken();
        string GetVault();
        void SetOrganization(string organization);
        void SetExecutionEnvironment(string? environment);
        void SetVault(string vault);
        void SetSessionToken(string sessionToken);
    }
}