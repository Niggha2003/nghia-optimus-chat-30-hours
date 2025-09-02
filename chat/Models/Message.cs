using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } 
        public int FromId { get; set; }
        public int ToId { get; set; }
        public bool IsGroup { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsViewed { get; set; } = false;
        public bool HasFile { get; set; } = false;
    }
}

