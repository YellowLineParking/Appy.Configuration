using System.Collections.Generic;

namespace Appy.Infrastructure.OnePassword.Model;

public class OnePasswordSection
{
    public string? Title { get; set; }

    public IReadOnlyCollection<OnePasswordInternalField>? Fields { get; set; }

    public OnePasswordSection WithTitle(string title)
    {
        Title = title;
        return this;
    }

    public OnePasswordSection WithFields(IReadOnlyCollection<OnePasswordInternalField> fields)
    {
        Fields = fields;
        return this;
    }
}