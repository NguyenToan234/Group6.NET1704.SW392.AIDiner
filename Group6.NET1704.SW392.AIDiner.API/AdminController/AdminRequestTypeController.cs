using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.Request;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Group6.NET1704.SW392.AIDiner.API.AdminController
{
    [Route("api/admin/request-types")]
    [ApiController]
    public class AdminRequestTypeController : ControllerBase
    {
        private IRequestTypeService _requestTypeService;

        public AdminRequestTypeController(IRequestTypeService requestTypeService)
        {
            _requestTypeService = requestTypeService;
        }
        [HttpGet]
        public async Task<ResponseDTO> GetAllRequestTypesForAdmin()
        {
            return await _requestTypeService.GetAllRequestTypesForAdmin();
        }
        [HttpPost]
        public async Task<ResponseDTO> CreateRequestTypeForAdmin([FromBody] string name)
        {
            return await _requestTypeService.CreateRequestTypeForAdmin(name);
        }
        [HttpPut("{id}")]
        public async Task<ResponseDTO> UpdateRequestTypeForAdmin(int id, [FromBody] string name)
        {
            return await _requestTypeService.UpdateRequestTypeForAdmin(id, name);
        }
        [HttpDelete("{id}")]
        public async Task<ResponseDTO> DeleteRequestTypeForAdmin(int id)
        {
            return await _requestTypeService.DeleteRequestTypeForAdmin(id);
        }

    }
}
