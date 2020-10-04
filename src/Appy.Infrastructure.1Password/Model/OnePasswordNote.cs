using System;
using System.Collections.Generic;
using System.Linq;

namespace Appy.Infrastructure.OnePassword.Model
{
    public class OnePasswordNote
    {
        public OnePasswordNoteDetails? Details { get; set; }

        public IList<OnePasswordSection>? GetSections() => Details?.Sections;

        public OnePasswordSection? GetSectionByEnvironment(string environment) =>
            GetSections()?
                .SingleOrDefault(section =>
                    string.Equals(section.Title, environment, StringComparison.InvariantCulture));
    }
}