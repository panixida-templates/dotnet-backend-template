//using Bl.Interfaces.Core;

//using Common.Constants;
//using Common.Constants.ApiEndpoints.Core;
//using Common.Storage.Dtos;

//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//using Pl.Api.Http.Dtos.Core;

//namespace Pl.Api.Http.Controllers.Core;

//[ApiController]
//public abstract class BaseStorageController(IBaseStorageBl storageBl) : ControllerBase
//{
//    [HttpGet(IBaseApiRoutesConstants.PresignedDownloadConstant)]
//    [ProducesResponseType(typeof(RestApiResponse<PresignedUrl>), StatusCodes.Status200OK)]
//    public async Task<ActionResult<PresignedUrl>> GetPresignedDownloadUrlAsync([FromQuery] string key)
//    {
//        var presignedUrl = await storageBl.GetPresignedDownloadUrlAsync(key);
//        return Ok(RestApiResponseBuilder<PresignedUrl>.Success(presignedUrl));
//    }

//    [HttpGet(IBaseApiRoutesConstants.PresignedUploadConstant)]
//    [ProducesResponseType(typeof(RestApiResponse<PresignedUrl>), StatusCodes.Status200OK)]
//    public async Task<ActionResult<PresignedUrl>> GetPresignedUploadUrlAsync([FromQuery] string fileName, [FromQuery] string? contentType = null)
//    {
//        var presignedUrl = await storageBl.GetPresignedUploadUrlAsync(fileName, contentType);
//        return Ok(RestApiResponseBuilder<PresignedUrl>.Success(presignedUrl));
//    }

//    [HttpGet(IBaseApiRoutesConstants.DownloadByKeyConstant)]
//    [ResponseCache(Duration = FilesConstants.ResponseCacheDurationSeconds)]
//    public async Task<IActionResult> DownloadAsync([FromRoute] string key, CancellationToken cancellationToken)
//    {
//        var fileContent = await storageBl.DownloadAsync(key, cancellationToken);
//        return File(fileContent.Content, fileContent.ContentType, fileContent.FileName, enableRangeProcessing: true);
//    }

//    [HttpPost(IBaseApiRoutesConstants.UploadConstant)]
//    [RequestFormLimits(MultipartBodyLengthLimit = FilesConstants.FileRequestSizeLimit)]
//    [ProducesResponseType(typeof(RestApiResponse<string>), StatusCodes.Status200OK)]
//    public async Task<ActionResult<string>> UploadAsync(IFormFile file, CancellationToken cancellationToken)
//    {
//        var fileContent = new FileContent(file.OpenReadStream(), file.ContentType, file.FileName);
//        var key = await storageBl.UploadAsync(fileContent, cancellationToken);
//        return Ok(RestApiResponseBuilder<string>.Success(key));
//    }

//    [HttpDelete(IBaseApiRoutesConstants.KeyConstant)]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    public async Task<IActionResult> DeleteAsync([FromRoute] string key, CancellationToken cancellationToken)
//    {
//        await storageBl.DeleteAsync(key, cancellationToken);
//        return NoContent();
//    }

//    [HttpPost(IBaseApiRoutesConstants.DeleteBatchConstant)]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    public async Task<IActionResult> DeleteAsync([FromBody] List<string> keys, CancellationToken cancellationToken)
//    {
//        await storageBl.DeleteAsync(keys, cancellationToken);
//        return NoContent();
//    }
//}
