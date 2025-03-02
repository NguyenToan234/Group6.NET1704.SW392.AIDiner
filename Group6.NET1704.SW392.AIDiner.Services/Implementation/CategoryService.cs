using Group6.NET1704.SW392.AIDiner.Common.DTO.BusinessCode;
using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Group6.NET1704.SW392.AIDiner.DAL;
using Group6.NET1704.SW392.AIDiner.DAL.Contract;
using System.Linq.Expressions;
using Group6.NET1704.SW392.AIDiner.DAL.Models;
using Group6.NET1704.SW392.AIDiner.Common.DTO.Request;
using Group6.NET1704.SW392.AIDiner.Common.DTO.AdminDTO;

namespace Group6.NET1704.SW392.AIDiner.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private IGenericRepository<Category> _categoryRepositoy;
        private IUnitOfWork _unitOfWork;

        public CategoryService(IGenericRepository<Category> categoryRepositoy, IUnitOfWork unitOfWork)
        {
            _categoryRepositoy = categoryRepositoy;
            _unitOfWork = unitOfWork;
        }

       

        public async Task<ResponseDTO> GetAllCategory()
        {
           ResponseDTO dto = new ResponseDTO();
            try
            {
                var category = await _categoryRepositoy.GetAllDataByExpression(null, 0, 0, null, true);
                dto.Data = category.Items.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,                   
                    //Status = c.Status,
                    Image = c.Image,
                }).ToList();
                dto.IsSucess = true;


            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
               
            }
            return dto;
        }

        public async Task<ResponseDTO> UpdateCategoryForAdmin(int id,UpdateCategoryForAdminDTO updateDTO)
        {
            ResponseDTO dto = new ResponseDTO();

            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(updateDTO.Name))
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.INVALID_INPUT;
                    dto.message = "The name category cannot be left blank.";
                    return dto;
                }

                // Kiểm tra trường IsDeleted không được null
                if (updateDTO.IsDeleted != true && updateDTO.IsDeleted != false)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.INVALID_INPUT;
                    dto.message = "IsDeleted must be true or false.";
                    return dto;
                }
                var category = await _categoryRepositoy.GetById(id);
                if (category == null)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    dto.message = "Id does not exist";
                    return dto;
                }

                category.Name = updateDTO.Name;
                category.IsDeleted = updateDTO.IsDeleted;

                await _categoryRepositoy.Update(category);
                await _unitOfWork.SaveChangeAsync();

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.CREATE_SUCCESS;
                dto.message = "Update Category Successfully";
                dto.Data = new
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsDelete = category.IsDeleted
                };
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.message = "Error: " + ex.Message;
            }
            return dto;
        }


        public async Task<ResponseDTO> CreateCategoryForAdmin(CreateDishCategoryForAdminDTO createDishCategoryForAdminDTO)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {

                var newCategory = new Category
                {
                    Name = createDishCategoryForAdminDTO.Name,

                };

                await _categoryRepositoy.Insert(newCategory);
                await _unitOfWork.SaveChangeAsync();


                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.CREATE_SUCCESS;
                dto.Data = new { newCategory.Id, newCategory.Name, newCategory.IsDeleted };
                dto.message = "Create Dish-Category Successfully";

            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.message = "Error" + ex.Message;
            }
            return dto;

        }

        public async Task<ResponseIsSucessDTO> DeleteCategoryForAdmin(int id)
        {
            ResponseIsSucessDTO dto = new ResponseIsSucessDTO();
            try
            {
                var category = await _categoryRepositoy.GetById(id);
                if (category == null)
                {
                    dto.IsSucess = false;
                 
                    return dto;
                }
                category.IsDeleted = true;
                await _categoryRepositoy.Update(category);
                await _unitOfWork.SaveChangeAsync();
                dto.IsSucess = true;
               
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
               
            }
           

            return dto;
        }
    }
}
