using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.Request;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Group6.NET1704.SW392.AIDiner.API.AdminController
{
    [Route("api/admin/dishes")]
    [ApiController]
    public class AdminDishController : ControllerBase
    {
        private IDishService _dishService;

        public AdminDishController(IDishService dishService)
        {
            _dishService = dishService;
        }
        [HttpGet]
        public async Task<ResponseDTO> GetDishesForAdmin(
            [FromQuery] int? category = null,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            return await _dishService.GetDishesForAdmin(category, page, size, search, sortBy, sortOrder);
        }
        [HttpPost]
        public async Task<ResponseDTO> CreateDish([FromBody] CreateDishDTO createDishDTO)
        {
            return await _dishService.CreateDish(createDishDTO);
        }
        [HttpPut("{id}")]
        public async Task<ResponseDTO> UpdateDish(int id, [FromBody] UpdateDishDTO updateDishDTO)
        {
            return await _dishService.UpdateDish(id, updateDishDTO);
        }
        [HttpDelete("{id}")]
        public async Task<ResponseDTO> DeleteDishForAdmin(int id)
        {
            return await _dishService.DeleteDishForAdmin(id);
        }

    }
}
