using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.Model.UserModel;
using Group6.NET1704.SW392.AIDiner.Common.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Contract
{
    public interface IUserService
    {
        public Task<ResponseDTO> GetAllUser(int page, int size, string? search, string? sortBy, string? sortOrder);
        public Task<ResponseDTO> GetUserById(int id);
        public Task<ResponseDTO> UpdateUserForAdmin(UpdateUserModel userDTO);
        public Task<ResponseDTO> UpdateProfileUser(int id, UpdateProfileUserModel userDTO);
        public Task<ResponseIsSucessDTO> DeleteUserForAdmin(int id);
    }
}
