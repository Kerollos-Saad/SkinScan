using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkinScan_API.Common;
using SkinScan_API.Dtos;
using SkinScan_Core.Contexts;
using SkinScan_Core.Entites;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SkinScan_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly SkinDbAppContext _context;

        public MessagesController(SkinDbAppContext context)
        {
            _context = context;
        }

        // GET: api/messages/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessage(int id)
        {
            var messageDto = await _context.Messages
                .Where(m => m.Id == id)
                .Select( m => new MessageDto
                {
                    Id = m.Id,
                    Text = m.Text,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId

                }).FirstOrDefaultAsync();

            if (messageDto == null)
                return NotFound();

            return Ok(ResponseModel<object>.SuccessResponse(messageDto, "Found Message", 200));
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage(AddMessageDto messageDto)
        {
	        var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(senderId == null)
	            return Unauthorized(ResponseModel<string>.ErrorResponse("Unauthorized User!", 401));


			var message = new Message
	        {
                SenderId = senderId,
                ReceiverId = messageDto.ReceiverId,
                Timestamp = DateTime.UtcNow,
                Text = messageDto.text
	        };

			try
			{
				await _context.Messages.AddAsync(message);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return BadRequest(ResponseModel<object>.ErrorResponse(ex.ToString()));
			}

			return Ok(ResponseModel<object>.SuccessResponse(new { MessageId = message.Id }, "Added Successfully"));
		}

        [HttpPut("{messageId}")]
        public async Task<IActionResult> PutMessage([FromRoute]int messageId, [FromBody]String messageText)
        {
			var existMessage = await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
			if (existMessage == null)
				return NotFound(ResponseModel<string>.ErrorResponse("Invalid Message Id", 400));

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (existMessage.SenderId != userId)
				return Unauthorized("Not Your Message");

			try
            {
	            existMessage.Timestamp = DateTime.UtcNow;
	            existMessage.Text = messageText;
				await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
	            return BadRequest(ResponseModel<object>.ErrorResponse(ex.ToString()));
            }

			return Ok(ResponseModel<object>.SuccessResponse(new { MessageId = messageId }, "Updated Successfully"));
        }

		[HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
	        var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
            if (message == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (message.SenderId != userId)
	            return Unauthorized("Not Your Message");

            try
            {
	            _context.Messages.Remove(message);
	            await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
	            return BadRequest(ResponseModel<object>.ErrorResponse(ex.ToString()));
            }

            return Ok(ResponseModel<object>.SuccessResponse(new { MessageId = messageId }, "Removed Successfully"));
        }

        [HttpGet("{targetUserId}/conversation")]
        public async Task<IActionResult> GetConversation(string targetUserId)
        {
	        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null)
		        return Unauthorized("Not Your Message");

			var messagesDto = await _context.Messages
		        .Where(m =>
			        (m.SenderId == userId && m.ReceiverId == targetUserId) ||
			        (m.SenderId == targetUserId && m.ReceiverId == userId))
		        .OrderBy(m => m.Timestamp)
		        .Select(m => new MessageDto
		        {
			        Id = m.Id,
			        Text = m.Text,
			        SenderId = m.SenderId,
			        ReceiverId = m.ReceiverId,
			        TimeStamp = m.Timestamp,
		        })
		        .ToListAsync();

	        return Ok(ResponseModel<object>.SuccessResponse(new { Messages = messagesDto }));
        }

        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation(string user1Id, string user2Id)
        {
	        var messagesDto = await _context.Messages
		        .Where(m =>
			        (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
			        (m.SenderId == user2Id && m.ReceiverId == user1Id))
		        .OrderBy(m => m.Timestamp)
		        .Select(m => new MessageDto
		        {
			        Id = m.Id,
			        Text = m.Text,
			        SenderId = m.SenderId,
			        ReceiverId = m.ReceiverId,
			        TimeStamp = m.Timestamp,
		        })
		        .ToListAsync();

	        return Ok(ResponseModel<object>.SuccessResponse(new { Messages = messagesDto }));
        }
	}
}
