﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Group6.NET1704.SW392.AIDiner.Common.Response;
using Microsoft.AspNetCore.Authorization;

namespace Group6.NET1704.SW392.AIDiner.API.UserControllers
{
    [Route("api/restaurants")]
    [ApiController]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private IRestaurantService _service;

        public RestaurantController(IRestaurantService service)
        {
            _service = service;
        }

        [HttpGet("tables")]

        public async Task<IActionResult> GetAllWithTablesAsync()
        {
            var response = await _service.GetAllWithTablesAsync();
            return Ok(response);
        }
    }
}
