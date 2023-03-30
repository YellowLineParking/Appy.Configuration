using System.Collections.Generic;

namespace Appy.Infrastructure.OnePassword.Model;

public class OnePasswordNoteDetails
{
    public IReadOnlyCollection<OnePasswordSection>? Sections { get; set; }

    public OnePasswordNoteDetails WithSections(IReadOnlyCollection<OnePasswordSection> sections)
    {
        Sections = sections;
        return this;
    }
}