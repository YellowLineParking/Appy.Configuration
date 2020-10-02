namespace Appy.Infrastructure.OnePassword.Model
{
    public class OnePasswordField
    {
        public string? Name => T;
        public string? Value => V;
        public string? T { get; set; }
        public string? V { get; set; }
    }
}
