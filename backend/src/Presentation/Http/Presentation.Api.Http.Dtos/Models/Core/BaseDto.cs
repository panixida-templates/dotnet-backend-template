namespace Presentation.Api.Http.Dtos.Models.Core;

public abstract record BaseDto<TId>
    where TId : struct
{
    public required TId Id { get; set; }
}
