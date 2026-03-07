namespace Presentation.Api.Http.Dtos.Models.Core;

public abstract record BaseDto<TId>
    where TId : struct
{
    public TId Id { get; set; }
}
