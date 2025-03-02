using Group6.NET1704.SW392.AIDiner.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Contract
{
    public interface IRequestService
    {
        Task<ResponseDTO> UpdateRequestStatus (int requestId, string status);
        Task<ResponseDTO> GetAllRequest();
        public Task<ResponseDTO> GetRequestByOrderID(int orderID);
        public Task<ResponseDTO> CreateRequest(CreateRequestDTO requestDto);
        public Task<ResponseDTO> GetRequests(int? restaurantId);
    }
}
