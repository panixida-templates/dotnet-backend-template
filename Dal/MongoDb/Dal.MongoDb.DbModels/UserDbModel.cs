using Common.Enums;

using Dal.MongoDb.DbModels.Core;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dal.MongoDb.DbModels;

public sealed class UserDbModel : BaseDbModel<Guid>
{
    [BsonRepresentation(BsonType.String)]
    public Role Role { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public int Age { get; set; }
    public DateTime Birthday { get; set; }
}
