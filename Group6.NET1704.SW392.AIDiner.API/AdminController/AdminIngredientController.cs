using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Group6.NET1704.SW392.AIDiner.API.AdminController
{
    [Route("api/admin/ingredients")]
    [ApiController]
    public class AdminIngredientController : ControllerBase
    {
        private IIngredientService _ingredientService;

        public AdminIngredientController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }
        [HttpGet]
        public async Task<ResponseDTO> GetAllIngredients()
        {
            return await _ingredientService.GetAllIngredients();
        }
        [HttpPost]
        public async Task<ResponseDTO> CreateIngredient([FromBody] CreateUpdateIngredientDTO createUpdateIngredientDTO)
        {
            return await _ingredientService.CreateIngredient(createUpdateIngredientDTO);
        }
        [HttpPut("{id}")]
        public async Task<ResponseDTO> UpdateIngredient(int id, [FromBody] CreateUpdateIngredientDTO createUpdateIngredientDTO)
        {
            return await _ingredientService.UpdateIngredient(id, createUpdateIngredientDTO);
        }
        [HttpDelete("{id}")]
        public async Task<ResponseDTO> DeleteIngredientForAdmin(int id)
        {
            return await _ingredientService.DeleteIngredientForAdmin(id);
        }
    }
}
