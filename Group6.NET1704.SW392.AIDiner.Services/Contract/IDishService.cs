using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.AdminDTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.Request;
using Group6.NET1704.SW392.AIDiner.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Contract
{
    public interface IDishService
    {
        public  Task<ResponseDTO> GetAllDishes();
        public Task<ResponseDTO> GetDishByIdAsync(int dishId);
        public Task<ResponseDTO> GetDishesForAdmin(int? categoryId, int page, int size, string? search, string? sortBy, string? sortOrder);
        public Task<ResponseDTO> CreateDish(CreateDishDTO createDishDTO);
        public Task<ResponseDTO> UpdateDish(int dishId ,UpdateDishDTO updateDishDTO);
        public Task<ResponseDTO> DeleteDishForAdmin(int id);
        public Task<ResponseDTO> GetDishesByRestaurantId(int restaurantId);
    }
}
