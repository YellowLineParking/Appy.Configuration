using Microsoft.AspNetCore.Mvc;

namespace Appy.Sample.OnePassword.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        public string Get() => "Hello!";
    }
}
