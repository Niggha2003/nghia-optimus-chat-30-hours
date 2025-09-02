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
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessagesController(IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor)
        {
            _messageRepository = messageRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("Group/{groupId}")]
        public async Task<IActionResult> GetMessagesOfAGroup(int groupId)
        {
            var messages = await _messageRepository.GetMessagesOfAGroupAsync(groupId);
            return Ok(messages);
        }

        [HttpGet("{fromId}/{toId}")]
        public async Task<IActionResult> GetMessages(int fromId, int toId, [FromQuery] bool isGroup)
        {
            var messages = await _messageRepository.GetMessagesAsync(fromId, toId, isGroup);
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessageById(int id)
        {
            var messages = await _messageRepository.GetMessageByIdAsync(id);
            if (messages == null)
            {
                return NotFound("Message not found.");
            }
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] Message message)
        {
            await _messageRepository.CreateMessageAsync(message);
            return CreatedAtAction(nameof(GetMessageById), new { id = message.Id }, message);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMessage([FromBody] Message message)
        {
            await _messageRepository.UpdateMessageAsync(message);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            await _messageRepository.DeleteMessageAsync(id);
            return NoContent();
        }
    }
}

