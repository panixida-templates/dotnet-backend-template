namespace Dal.MongoDb.DbModels.Core;

public abstract class BaseDbModel<TId>
    where TId : struct
{
    public TId Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
