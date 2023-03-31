namespace Appy.Infrastructure.OnePassword.Queries;

public class FetchOnePasswordNoteQuery
{
    public string Item { get; set; }
    public string Vault { get; set; }
    public string UserId { get; set; }
    public string Environment { get; set; }
    public string SessionToken { get; set; }
}