using Appy.Infrastructure.OnePassword.Model;
using Spectre.Console;

namespace Appy.Tool.OnePassword.Logging;

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
            .AddRow(nameof(AppyOnePasswordSession.Organization), session.Organization!)
            .AddRow(nameof(AppyOnePasswordSession.Vault), session.Vault!)
            .AddRow(nameof(AppyOnePasswordSession.Environment), session.Environment!)
            .AddRow(nameof(AppyOnePasswordSession.SessionToken), session.SessionToken!)
            .AsciiBorder()
            .HideHeaders();

        AnsiConsole.Write(headerTable);
        AnsiConsole.Write(sessionTable);
    }
}