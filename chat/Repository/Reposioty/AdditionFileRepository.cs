using Chat.Models;
using Chat.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Repository
{
    public class AdditionFileRepository : IAdditionFileRepository
    {
        private readonly IMongoCollection<AdditionFile> _additionFileCollection;
        private readonly string _databaseName;

        public AdditionFileRepository(IMongoClient mongoClient, IConfiguration configuration)
        {
            _databaseName = configuration.GetValue<string>("MongoDbSettings:DatabaseName");  // Lấy tên database từ appsettings.json
            var database = mongoClient.GetDatabase(_databaseName);
            _additionFileCollection = database.GetCollection<AdditionFile>("AdditionFiles");
        }

        public async Task<List<AdditionFile>> GetFilesByMessageIdAsync(string messageId)
        {
            var filter = Builders<AdditionFile>.Filter.Eq(a => a.MessageId, messageId);
            return await _additionFileCollection.Find(filter).ToListAsync();
        }

        public async Task<AdditionFile> GetFileByIdAsync(string id)
        {
            var filter = Builders<AdditionFile>.Filter.Eq(a => a.Id, id);
            return await _additionFileCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAdditionFileAsync(AdditionFile additionFile)
        {
            // Tạo thư mục Uploads nếu chưa có
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            var guidStr = Guid.NewGuid().ToString();
            // Đường dẫn file
            var path = Path.Combine(uploadDir, additionFile.FileName + "_" + guidStr);

            // Chuyển base64 thành byte
            var bytes = Convert.FromBase64String(additionFile.FileBase64Content);

            // Ghi file an toàn với FileStream + using
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await fs.WriteAsync(bytes, 0, bytes.Length);
            }

            // Lưu URL
            additionFile.FileUrl = path;

            // Lưu thông tin vào MongoDB
            await _additionFileCollection.InsertOneAsync(additionFile);
        }

        public async Task UpdateAdditionFileAsync(AdditionFile additionFile)
        {
            var filter = Builders<AdditionFile>.Filter.Eq(a => a.Id, additionFile.Id);
            await _additionFileCollection.ReplaceOneAsync(filter, additionFile);
        }

        public async Task DeleteAdditionFileAsync(string id)
        {
            var filter = Builders<AdditionFile>.Filter.Eq(a => a.Id, id);
            await _additionFileCollection.DeleteOneAsync(filter);
        }
    }

}
