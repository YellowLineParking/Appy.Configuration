namespace Appy.Infrastructure.OnePassword.Queries
{
    public class OnePasswordGetNoteQuery
    {
        public string Item { get; set; }
        public string Vault { get; set; }
        public string Organisation { get; set; }
        public string Environment { get; set; }
        public string SessionToken { get; set; }
    }
}