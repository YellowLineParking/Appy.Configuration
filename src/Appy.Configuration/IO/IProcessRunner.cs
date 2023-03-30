using System.Threading.Tasks;

namespace Appy.Configuration.IO;

public interface IProcessRunner
{
    Task<ProcessResult> Run(string toolPath, ProcessSettings settings);
}