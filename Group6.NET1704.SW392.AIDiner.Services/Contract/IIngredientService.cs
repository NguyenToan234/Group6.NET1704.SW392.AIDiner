using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Group6.NET1704.SW392.AIDiner.Common.DTO;

namespace Group6.NET1704.SW392.AIDiner.Services.Contract
{
    public interface IIngredientService
    {
        public Task<ResponseDTO> GetAllIngredients();
        public Task<ResponseDTO> CreateIngredient(CreateUpdateIngredientDTO createUpdateIngredientDTO);
        public Task<ResponseDTO> UpdateIngredient(int id, CreateUpdateIngredientDTO createUpdateIngredientDTO);
        public Task<ResponseDTO> DeleteIngredientForAdmin(int id);
    }
}
