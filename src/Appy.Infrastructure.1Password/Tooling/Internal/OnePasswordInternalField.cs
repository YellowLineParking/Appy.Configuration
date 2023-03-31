using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Tooling.Internal;

public class OnePasswordInternalField
{
    public string? Label { get; set; }
    public string? Value { get; set; }
    public OnePasswordInternalSection Section { get; set; }

    public static OnePasswordInternalField New(string label, string value, OnePasswordInternalSection section) =>
        new() { Label = label, Value = value, Section = section,  };
}