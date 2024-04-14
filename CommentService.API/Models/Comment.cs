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
        public string? ThreadName { get; set; }
        public string? PostId { get; set; }
        public string? PostName { get; set; } //????
        public int AuthorId { get; set; } = 0;
        public string? AuthorName { get; set; } 
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
