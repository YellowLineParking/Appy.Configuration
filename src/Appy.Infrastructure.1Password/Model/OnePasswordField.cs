namespace Appy.Infrastructure.OnePassword.Model
{
    public class OnePasswordField
    {
        public string? Name { get; set; }
        public string? Value { get; set; }

        public static OnePasswordField New(string name, string value)
        {
            return new OnePasswordField { Name = name, Value = value };
        }
    }
}
