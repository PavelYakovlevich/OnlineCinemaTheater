using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Comment.Data.Entities;

public class Comment
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid MediaId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    public string Text { get; set; }

    public DateTime IssueDate { get; set; }

    public User User { get; set; }
}
