using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Group6.NET1704.SW392.AIDiner.API.UserControllers
{
    [Route("api/request-types")]
    [ApiController]
    public class RequestTypeController : ControllerBase
    {
        private IRequestTypeService _requestTypeService;

        public RequestTypeController(IRequestTypeService requestTypeService)
        {
            _requestTypeService = requestTypeService;
        }
        [HttpGet]
        public async Task<ResponseDTO> GetAllRequestTypes()
        {
            return await _requestTypeService.GetAllRequestTypes();
        }
    }
}
