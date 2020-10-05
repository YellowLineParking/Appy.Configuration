using System.IO;
using Appy.Tool.OnePassword.CLI;
using Spectre.Console;

namespace Appy.Tool.OnePassword.Logging
{
    internal class ConsoleVisualizer : IConsoleVisualzer
    {
        public void Render(AppyOnePasswordSession session)
        {
            var headerTable = new Table()
                .AddColumn(new TableColumn(string.Empty))
                .AddEmptyRow()
                .AddRow("Appy 1Password session started:")
                .NoBorder()
                .HideHeaders();

            var sessionTable = new Table()
                .AddColumn(new TableColumn(string.Empty))
                .AddColumn(new TableColumn(string.Empty))
                .AddRow(nameof(AppyOnePasswordSession.Organization), session.Organization)
                .AddRow(nameof(AppyOnePasswordSession.Vault), session.Vault)
                .AddRow(nameof(AppyOnePasswordSession.Environment), session.Environment)
                .AddRow(nameof(AppyOnePasswordSession.SessionToken), session.SessionToken)
                .AsciiBorder()
                .HideHeaders();

            var footerTable = new Table()
                .AddColumn(new TableColumn(string.Empty))
                .AddEmptyRow()
                .AddRow("You can now go to your project and start your debug session.")
                .AddRow("Session tokens expire after 30 minutes of inactivity, after which you’ll need to sign in again.")
                .AddEmptyRow()
                .NoBorder()
                .HideHeaders();

            AnsiConsole.Render(headerTable);
            AnsiConsole.Render(sessionTable);
            AnsiConsole.Render(footerTable);
        }
    }
}