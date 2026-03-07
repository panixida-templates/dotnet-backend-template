namespace Infrastructure.Persistence.Ef.Core;

internal abstract class DbModel<TId>
    where TId : struct
{
    public TId Id { get; set; }
}
