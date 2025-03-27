﻿using Group6.NET1704.SW392.AIDiner.Common.Request;
using Group6.NET1704.SW392.AIDiner.Common.Response;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Group6.NET1704.SW392.AIDiner.Services.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Group6.NET1704.SW392.AIDiner.API.UserControllers
{
    [Route("api/gemini")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
        private readonly GeminiService _geminiService;
        public GeminiController(GeminiService service)
        {
            _geminiService = service;
        }

        [HttpPost("message")]
        public async Task<IActionResult> ProcessMessage([FromBody] GeminiRequest request)
        {
            if (string.IsNullOrEmpty(request?.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            // 1. Gọi Gemini để phân tích tin nhắn
            GeminiResponse geminiResponse = await _geminiService.ProcessUserMessage(request.Message, request.orderId);

            if (geminiResponse == null)
            {
                return StatusCode(500, "Failed to process message with Gemini."); // Xử lý lỗi Gemini
            }

            // 2. Xử lý ý định
            switch (geminiResponse.Intent)
            {
                case "OrderFood":

                    var reponse = await _geminiService.OrderFood(geminiResponse);

                    return Ok(new { Message = reponse }); // Sử dụng ResponseText (hoặc thuộc tính phù hợp)

                    break;


                default:  
                    // Trường hợp này xử lý tất cả các Intent khác (bao gồm cả các câu hỏi thông tin)
                    // Trả về phản hồi trực tiếp từ Gemini
                    return Ok(new { Message = geminiResponse.ResponseText }); // Sử dụng ResponseText (hoặc thuộc tính phù hợp)
            }
        }


    }
}
