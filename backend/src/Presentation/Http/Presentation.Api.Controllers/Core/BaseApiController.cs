using Microsoft.AspNetCore.Mvc;

using System.Net.Mime;

namespace Presentation.Api.Controllers.Core;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public abstract class BaseApiController : ControllerBase
{
}
