using System;
using System.Collections.Generic;
using System.Linq;

namespace Appy.Infrastructure.OnePassword.Model
{
    public class OnePasswordNote
    {
        public OnePasswordNote WithDetails(OnePasswordNoteDetails details)
        {
            Details = details;
            return this;
        }

        IReadOnlyCollection<OnePasswordSection>? GetSections() => Details?.Sections;

        public OnePasswordNoteDetails? Details { get; set; }

        public OnePasswordSection? GetSectionByEnvironment(string environment)
        {
            return GetSections()?.SingleOrDefault(section =>
                string.Equals(section.Title, environment, StringComparison.InvariantCulture));
        }
    }
}