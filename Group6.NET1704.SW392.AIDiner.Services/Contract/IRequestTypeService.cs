using Group6.NET1704.SW392.AIDiner.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Contract
{
    public interface IRequestTypeService
    {
        Task<ResponseDTO> GetAllRequestTypes();
        Task<ResponseDTO> GetAllRequestTypesForAdmin();
        Task<ResponseDTO> CreateRequestTypeForAdmin(string name);
        Task<ResponseDTO> UpdateRequestTypeForAdmin(int id, string name);
        Task<ResponseDTO> DeleteRequestTypeForAdmin(int id);
    }
}
