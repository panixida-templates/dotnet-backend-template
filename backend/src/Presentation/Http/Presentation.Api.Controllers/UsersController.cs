using Application.Abstractions.Mediator;
using Application.Contracts.Messages.Users.Commands;
using Application.Contracts.Messages.Users.Queries;

using Common.Constants.ApiEndpoints;
using Common.Constants.ApiEndpoints.Core;
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
    public async Task<ActionResult<RestApiResponse<UserDto>>> Get([FromRoute] Guid id)
    {
        var dto = UsersMapper.ToDto(await mediator.QueryAsync(new GetUserQuery(id)));
        return Ok(RestApiResponseBuilder<UserDto>.Success(dto));
    }

    [HttpGet]
    [Route(IBaseApiRoutesConstants.GetByFilterSuffixConstant)]
    [ProducesResponseType(typeof(RestApiResponse<SearchResult<UserDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<SearchResult<UserDto>>>> Get([FromQuery] UsersSearchParams searchParams)
    {
        var searchResult = (await mediator.QueryAsync(new SearchUsersQuery(searchParams))).Map(UsersMapper.ToDto);
        return Ok(RestApiResponseBuilder<SearchResult<UserDto>>.Success(searchResult));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RestApiResponse<Guid>), StatusCodes.Status201Created)]
    public async Task<ActionResult<RestApiResponse<Guid>>> Create([FromBody] UserDto dto)
    {
        var id = await mediator.SendAsync(new CreateUserCommand(dto.Id, dto.Name, dto.Email, dto.Phone, dto.Birthday, dto.AvatarKey));
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, RestApiResponseBuilder<Guid>.Success(id));
    }

    [HttpPut]
    [Route(UsersApiEndpointsConstants.IdConstant)]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Update([FromRoute] Guid id, [FromBody] UserDto dto)
    {
        await mediator.SendAsync(new UpdateUserCommand(id, dto.Name, dto.Email, dto.Phone, dto.Birthday, dto.AvatarKey));
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
