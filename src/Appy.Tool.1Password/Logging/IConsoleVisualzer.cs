using Appy.Infrastructure.OnePassword.Model;

namespace Appy.Tool.OnePassword.Logging;

public interface IConsoleVisualzer
{
    void Render(AppyOnePasswordSession session);
}