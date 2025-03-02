using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.AdminDTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Contract
{
    public interface ITableService
    {
        //public Task<ResponseDTO> GetAllTable();
        public Task<ResponseDTO> GetAllTableForAdmin();
        public Task<ResponseDTO> CreateTableForAdmin(CreateTableForAdminDTO createTableDTO);
        public Task<ResponseDTO> UpdateTableForAdmin(int id, UpdateTableRequest updateRequest);
        public Task<ResponseIsSucessDTO> DeleteTableForAdmin(int id);
    }
}
