namespace Infrastructure.Ef.Core.Read;

internal abstract class ReadDbModel<TId>
    where TId : struct
{
    public TId Id { get; set; }
}
