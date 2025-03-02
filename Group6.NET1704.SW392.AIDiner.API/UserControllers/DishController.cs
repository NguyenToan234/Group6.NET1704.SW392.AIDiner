using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.DAL.Models;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Group6.NET1704.SW392.AIDiner.API.Controllers
{
    [Route("api/dishes")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }
        [HttpGet]
        public async Task<ResponseDTO> GetAllDishes() 
        { 
        return await _dishService.GetAllDishes();
        }
        [HttpGet("{id}")]
        public async Task<ResponseDTO> GetDishById(int id)
        {
            return await _dishService.GetDishByIdAsync(id);
        }

        [HttpGet("a")]
        public async Task<ResponseDTO> GetDishesByRestaurantId(int restaurantId)
        {
            return await _dishService.GetDishesByRestaurantId(restaurantId);
        }
    }
}
