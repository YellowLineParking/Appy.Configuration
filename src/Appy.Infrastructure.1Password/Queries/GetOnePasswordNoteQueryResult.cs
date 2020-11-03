using System.Collections.Generic;
using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Infrastructure.OnePassword.Queries
{
    public class GetOnePasswordNoteQueryResult
    {
        public string Title { get; set; }

        public IReadOnlyCollection<OnePasswordField>? Fields { get; set; }

        public GetOnePasswordNoteQueryResult WithTitle(string title)
        {
            Title = title;
            return this;
        }

        public GetOnePasswordNoteQueryResult WithFields(IReadOnlyCollection<OnePasswordField> fields)
        {
            Fields = fields;
            return this;
        }
    }
}