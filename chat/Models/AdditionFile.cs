using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Models
{
    public class AdditionFile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string MessageId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string FileBase64Content { get; set; }
    }
}
