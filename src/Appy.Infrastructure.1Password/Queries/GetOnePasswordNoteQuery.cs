namespace Appy.Infrastructure.OnePassword.Queries
{
    public class GetOnePasswordNoteQuery
    {
        public string Item { get; set; }
        public string Vault { get; set; }
        public string Organization { get; set; }
        public string Environment { get; set; }
        public string SessionToken { get; set; }
    }
}