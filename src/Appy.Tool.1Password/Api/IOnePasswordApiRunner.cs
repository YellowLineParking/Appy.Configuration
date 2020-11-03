using System.Threading.Tasks;

namespace Appy.Tool.OnePassword.Api
{
    public interface IOnePasswordApiRunner
    {
        void Start(OnePasswordApiSettings settings);
        bool IsRunning();
        Task Stop();
    }
}