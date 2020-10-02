using System.Collections.Generic;

namespace Appy.Infrastructure.OnePassword.Model
{
    public class OnePasswordSection
    {
        public string? Title { get; set; }

        public IList<OnePasswordField>? Fields { get; set; }
    }
}