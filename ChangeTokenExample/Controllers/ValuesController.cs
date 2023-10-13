using Microsoft.AspNetCore.Mvc;

namespace ChangeTokenTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private FileCache _fileCache;

        public ValuesController(FileCache fileCache)
        {
            _fileCache = fileCache;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            var value = await _fileCache.GetFileContents("appsettings.json");
            return value;
        }
    }
}
