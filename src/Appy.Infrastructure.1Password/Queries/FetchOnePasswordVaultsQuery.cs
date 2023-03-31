namespace Appy.Infrastructure.OnePassword.Queries;

public class FetchOnePasswordVaultsQuery
{
    public string UserId { get; set; }
    public string SessionToken { get; set; }
}