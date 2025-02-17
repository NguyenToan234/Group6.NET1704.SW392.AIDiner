﻿using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Group6.NET1704.SW392.AIDiner.API.AdminController
{
    [Route("api/admin")]
    [ApiController]
    public class AdminForTableController : ControllerBase
    {
        private ITableService _service;

        public AdminForTableController(ITableService service)
        {
            _service = service;
        }

        [HttpGet("tables")]
        public async Task<ResponseDTO> GetAllTable()
        {
            return await _service.GetAllTable();
        }
    }
}
