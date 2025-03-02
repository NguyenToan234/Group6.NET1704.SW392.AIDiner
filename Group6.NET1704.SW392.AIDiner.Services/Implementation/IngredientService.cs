using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.BusinessCode;
using Group6.NET1704.SW392.AIDiner.DAL.Contract;
using Group6.NET1704.SW392.AIDiner.DAL.Implementation;
using Group6.NET1704.SW392.AIDiner.DAL.Models;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Implementation
{
    public class IngredientService : IIngredientService
    {
        private IUnitOfWork _unitOfWork;
        private IGenericRepository<Ingredient> _ingredientRepository;

        public IngredientService(IUnitOfWork unitOfWork, IGenericRepository<Ingredient> ingredientRepository)
        {
            _unitOfWork = unitOfWork;
            _ingredientRepository = ingredientRepository;
        }

        public async Task<ResponseDTO> CreateIngredient(CreateUpdateIngredientDTO createUpdateIngredientDTO)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var newIngredient = new Ingredient
                {
                    Name = createUpdateIngredientDTO.Name,
                    Image = createUpdateIngredientDTO.Image,
                };
                await _ingredientRepository.Insert(newIngredient);
                await _unitOfWork.SaveChangeAsync();
                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.CREATE_SUCCESS;
                dto.Data = new { newIngredient.Id, newIngredient.Name, };
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.Data = ex.Message;
            }
            return dto;
        }

        public async Task<ResponseDTO> DeleteIngredientForAdmin(int id)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var ingredient = await _ingredientRepository.GetById(id);
                if (ingredient == null)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    dto.Data = "Không tìm thấy nguyên liệu.";
                    return dto;
                }
                ingredient.IsDeleted = true;
                await _ingredientRepository.Update(ingredient);
                await _unitOfWork.SaveChangeAsync(); 

                dto.IsSucess = true;
                dto.BusinessCode= BusinessCode.DELETE_SUCCESS;
                dto.Data = new {ingredient.Id,ingredient.IsDeleted, };
                }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.Data = ex.Message;
            }
            return dto;
        }

        public async Task<ResponseDTO> GetAllIngredients()
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var ingredients = await _ingredientRepository.GetAllDataByExpression(null, 0, 0, null, true);
                dto.Data = ingredients.Items.Select(a => new IngredientDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Image = a.Image,

                }).ToList();
                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
            }
            return dto;
        }

        public async Task<ResponseDTO> UpdateIngredient(int id, CreateUpdateIngredientDTO createUpdateIngredientDTO)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var ingredient = await _ingredientRepository.GetById(id);
                if (ingredient == null)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    return dto;
                }
                ingredient.Name = createUpdateIngredientDTO.Name;
                ingredient.Image = createUpdateIngredientDTO.Image;
                 await _ingredientRepository.Update(ingredient);
                await _unitOfWork.SaveChangeAsync();
                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.UPDATE_SUCESSFULLY;
                dto.Data = new { ingredient.Id, ingredient.Name, ingredient.Image, };

            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode=BusinessCode.EXCEPTION;
                dto.Data = ex.Message;
            }
            return dto;
        }
    }
}
