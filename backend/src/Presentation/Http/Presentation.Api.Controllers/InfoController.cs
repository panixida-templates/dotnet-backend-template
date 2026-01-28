using Microsoft.AspNetCore.Mvc;

using Presentation.Api.Controllers.Core;
using Presentation.Api.Http.Dtos.Core;

namespace Presentation.Api.Controllers;

[Route("api/info")]
public sealed class InfoController : BaseApiController
{
    [HttpGet]
    [Route("version")]
    [ProducesResponseType(typeof(RestApiResponse<string>), 200)]
    public IActionResult GetVersion()
    {
        return Ok(RestApiResponseBuilder<string>.Success(Environment.GetEnvironmentVariable("API_VERSION") ?? "1.0"));
    }
}
