using System.Collections.Generic;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Queries;

public class FetchOnePasswordNoteQueryResult
{
    public string Title { get; set; }
    public IReadOnlyCollection<OnePasswordField>? Fields { get; set; }

    public FetchOnePasswordNoteQueryResult WithTitle(string title)
    {
        Title = title;
        return this;
    }

    public FetchOnePasswordNoteQueryResult WithFields(IReadOnlyCollection<OnePasswordField> fields)
    {
        Fields = fields;
        return this;
    }
}