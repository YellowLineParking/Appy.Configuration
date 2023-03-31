using System;
using System.Collections.Generic;
using System.Linq;

namespace Appy.Infrastructure.OnePassword.Tooling.Internal;

public class OnePasswordInternalNote
{
    public IReadOnlyCollection<OnePasswordInternalField> Fields { get; set; }

    public IEnumerable<OnePasswordInternalField>? GetFieldsForEnvironment(string environment)
    {
        return Fields?.Where(field =>
            !string.IsNullOrEmpty(field.Section?.Label) &&
            string.Equals(field.Section.Label, environment, StringComparison.InvariantCulture));
    }
}