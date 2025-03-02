using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.AdminDTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.Request;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Group6.NET1704.SW392.AIDiner.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
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

        //[HttpGet("tables")]
        //public async Task<ResponseDTO> GetAllTable()
        //{
        //    return await _service.GetAllTable();
        //}

        [HttpGet("tables")]
        public async Task<ResponseDTO> GetAllTableForAdmin()
        {
            return await _service.GetAllTableForAdmin();
        }

        [HttpPost("tables")]

        public async Task<ResponseDTO> CreateTableForAdmin(CreateTableForAdminDTO createTableDTO)
        {
            return await _service.CreateTableForAdmin(createTableDTO);

        }
        [HttpPut("{id}")]
        public async Task<ResponseDTO> UpdateTable(int id, [FromBody] UpdateTableRequest updateRequest)
        {
            return await _service.UpdateTableForAdmin(id, updateRequest);
        }

        [HttpDelete("tables/{id}")]
        public async Task<ResponseIsSucessDTO> DeleteTableForAdmin(int id)
        {
            return await (_service.DeleteTableForAdmin(id));
        }  
    }
}
