
using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.AdminDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Contract
{
    public interface ICategoryService
    {
        public Task<ResponseDTO> GetAllCategory();
        public Task<ResponseDTO> CreateCategoryForAdmin(CreateDishCategoryForAdminDTO createDishCategoryForAdminDTO);
        public Task<ResponseDTO> UpdateCategoryForAdmin(int id,UpdateCategoryForAdminDTO updateDTO);
        public Task<ResponseIsSucessDTO> DeleteCategoryForAdmin(int id);


    }
}
