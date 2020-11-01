using Appy.Infrastructure.OnePassword.Model;
using Appy.Tool.OnePassword.CLI;

namespace Appy.Tool.OnePassword.Logging
{
    public interface IConsoleVisualzer
    {
        void Render(AppyOnePasswordSession session);
    }
}