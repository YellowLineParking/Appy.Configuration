namespace Appy.Infrastructure.OnePassword.Model;

public class OnePasswordField
{
    public string? Name { get; set; }
    public string? Value { get; set; }

    public static OnePasswordField New(string name, string value) => new() { Name = name, Value = value };
}