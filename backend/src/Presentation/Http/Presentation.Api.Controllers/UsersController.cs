using Application.Abstractions.Mediator;
using Application.Users.Create;
using Application.Users.Delete;
using Application.Users.Get;
using Application.Users.Search;
using Application.Users.Update;

using Common.Constants.ApiEndpoints;
using Common.Constants.ApiEndpoints.Core;
using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Presentation.Api.Controllers.Core;
using Presentation.Api.Http.Dtos.Core;
using Presentation.Api.Http.Dtos.Models;
using Presentation.Api.Http.Mappers;

namespace Presentation.Api.Controllers;

[Route(UsersApiEndpointsConstants.BaseConstant)]
public sealed class UsersController(IMediator mediator) : BaseApiController
{
    [HttpGet]
    [Route(UsersApiEndpointsConstants.IdConstant)]
    [ProducesResponseType(typeof(RestApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestApiResponse<UserDto>>> Get([FromRoute] Guid id, [FromQuery] UsersConvertParams? convertParams)
    {
        var dto = UsersMapper.ToDto(await mediator.QueryAsync(new GetUserQuery(id, convertParams)));
        return Ok(RestApiResponseBuilder<UserDto>.Success(dto));
    }

    [HttpGet]
    [Route(IBaseApiRoutesConstants.GetByFilterSuffixConstant)]
    [ProducesResponseType(typeof(RestApiResponse<SearchResult<UserDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<SearchResult<UserDto>>>> Get([FromQuery] UsersSearchParams searchParams, [FromQuery] UsersConvertParams? convertParams)
    {
        var searchResult = (await mediator.QueryAsync(new SearchUsersQuery(searchParams, convertParams))).Map(UsersMapper.ToDto);
        return Ok(RestApiResponseBuilder<SearchResult<UserDto>>.Success(searchResult));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RestApiResponse<Guid>), StatusCodes.Status201Created)]
    public async Task<ActionResult<RestApiResponse<Guid>>> Create([FromBody] UserDto dto)
    {
        var id = await mediator.SendAsync(new CreateUserCommand(UsersMapper.ToEntity(dto)));
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, RestApiResponseBuilder<Guid>.Success(id));
    }

    [HttpPut]
    [Route(UsersApiEndpointsConstants.IdConstant)]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Update([FromRoute] Guid id, [FromBody] UserDto dto)
    {
        await mediator.SendAsync(new UpdateUserCommand(id, UsersMapper.ToEntity(dto)));
        return Ok(RestApiResponseBuilder<NoContent>.Success(new()));
    }

    [HttpDelete]
    [Route(UsersApiEndpointsConstants.IdConstant)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await mediator.SendAsync(new DeleteUserCommand(id));
        return NoContent();
    }
}
