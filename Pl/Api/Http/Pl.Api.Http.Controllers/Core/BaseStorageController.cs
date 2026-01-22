using Bl.Interfaces.Core;

using Dto.Storage;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Pl.Api.Http.Dtos.Core;

namespace Pl.Api.Http.Controllers.Core;

[ApiController]
public abstract class BaseStorageController(IBaseStorageBl storageBl) : ControllerBase
{
    [HttpGet("presigned/upload")]
    [ProducesResponseType(typeof(RestApiResponse<PresignedUrl>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PresignedUrl>> GetPresignedUploadUrlAsync([FromQuery] string fileName, [FromQuery] string? contentType = null)
    {
        var presignedUrl = await storageBl.GetPresignedUploadUrlAsync(fileName, contentType);
        return Ok(RestApiResponseBuilder<PresignedUrl>.Success(presignedUrl));
    }

    [HttpGet("presigned/download")]
    [ProducesResponseType(typeof(RestApiResponse<PresignedUrl>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PresignedUrl>> GetPresignedDownloadUrlAsync([FromQuery] string key)
    {
        var presignedUrl = await storageBl.GetPresignedDownloadUrlAsync(key);
        return Ok(RestApiResponseBuilder<PresignedUrl>.Success(presignedUrl));
    }

    [HttpGet("download/{*key}")]
    [ResponseCache(Duration = 60 * 60 * 24)]
    public async Task<IActionResult> DownloadAsync([FromRoute] string key, CancellationToken cancellationToken)
    {
        var fileContent = await storageBl.DownloadAsync(key, cancellationToken);
        return File(fileContent.Content, fileContent.ContentType, fileContent.FileName, enableRangeProcessing: true);
    }

    [HttpPost("upload")]
    [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
    [ProducesResponseType(typeof(RestApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var fileContent = new FileContent(file.OpenReadStream(), file.ContentType, file.FileName);
        var key = await storageBl.UploadAsync(fileContent, cancellationToken);
        return Ok(RestApiResponseBuilder<string>.Success(key));
    }

    [HttpDelete("{*key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync([FromRoute] string key, CancellationToken cancellationToken)
    {
        await storageBl.DeleteAsync(key, cancellationToken);
        return NoContent();
    }

    [HttpPost("delete-batch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync([FromBody] List<string> keys, CancellationToken cancellationToken)
    {
        await storageBl.DeleteAsync(keys, cancellationToken);
        return NoContent();
    }
}
