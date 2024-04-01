using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CommentService.API.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? ThreadId { get; set; }
        public string? PostId { get; set; }
        public string? AuthorId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
