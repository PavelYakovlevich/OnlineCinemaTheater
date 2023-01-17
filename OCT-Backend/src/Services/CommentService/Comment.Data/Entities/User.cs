using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Comment.Data.Entities;

public class User
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }
}
