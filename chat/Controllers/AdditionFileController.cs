using Chat.Models;
using Chat.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdditionFileController : ControllerBase
    {
        private readonly IAdditionFileRepository _additionFileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdditionFileController(IAdditionFileRepository additionFileRepository, IHttpContextAccessor httpContextAccessor)
        {
            _additionFileRepository = additionFileRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/AdditionFile/Message/{messageId}
        [HttpGet("Message/{messageId}")]
        public async Task<IActionResult> GetFilesByMessageId(string messageId)
        {
            var files = await _additionFileRepository.GetFilesByMessageIdAsync(messageId);
            if (files == null || files.Count == 0)
            {
                return NotFound("No files found for this message.");
            }
            return Ok(files);
        }

        // GET: api/AdditionFile/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(string id)
        {
            // Lấy userId từ JWT
            var userId = User.Identity?.Name;

            // Lấy thông tin file từ DB
            var file = await _additionFileRepository.GetFileByIdAsync(id);
            if (file == null) return NotFound();

            //// Kiểm tra quyền truy cập
            //if (!_additionFileRepository.UserHasAccess(userId, fileId))
            //    return Forbid(); // 403 nếu không có quyền

            // Lấy file từ server
            var path = Path.Combine("", file.FileUrl);
            if (!System.IO.File.Exists(path)) return NotFound();

            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", file.FileName);
        }

        // POST: api/AdditionFile
        [HttpPost]
        public async Task<IActionResult> CreateAdditionFile([FromBody] AdditionFile additionFile)
        {
            if (additionFile == null || string.IsNullOrEmpty(additionFile.FileBase64Content))
            {
                return BadRequest("Invalid file data.");
            }

            await _additionFileRepository.CreateAdditionFileAsync(additionFile);
            return CreatedAtAction(nameof(GetFileById), new { id = additionFile.Id }, additionFile);
        }

        // PUT: api/AdditionFile
        [HttpPut]
        public async Task<IActionResult> UpdateAdditionFile([FromBody] AdditionFile additionFile)
        {
            if (additionFile == null || string.IsNullOrEmpty(additionFile.FileBase64Content))
            {
                return BadRequest("Invalid file data.");
            }

            var existingFile = await _additionFileRepository.GetFileByIdAsync(additionFile.Id);
            if (existingFile == null)
            {
                return NotFound("File not found.");
            }

            await _additionFileRepository.UpdateAdditionFileAsync(additionFile);
            return NoContent();
        }

        // DELETE: api/AdditionFile/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdditionFile(string id)
        {
            var file = await _additionFileRepository.GetFileByIdAsync(id);
            if (file == null)
            {
                return NotFound("File not found.");
            }

            await _additionFileRepository.DeleteAdditionFileAsync(id);
            return NoContent();
        }
    }
}

