using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.DAL.Contract;
using Group6.NET1704.SW392.AIDiner.DAL.Models;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Group6.NET1704.SW392.AIDiner.Common.DTO.BusinessCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Implementation
{
    public class RequestTypeService : IRequestTypeService
    {
        private IGenericRepository<RequestType> _requestTypeRepository;
        private IUnitOfWork _unitOfWork;

        public RequestTypeService(IGenericRepository<RequestType> requestTypeRepository, IUnitOfWork unitOfWork)
        {
            _requestTypeRepository = requestTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO> CreateRequestTypeForAdmin(string name)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.INVALID_INPUT;
                    dto.Data = "Tên loại yêu cầu không được để trống.";
                    return dto;
                }
                var existingType = await _requestTypeRepository.GetByExpression(rt => rt.Name == name);
                if (existingType != null)
                {
                    dto.IsSucess = false;
                    dto.Data = "Tên loại yêu cầu đã tồn tại.";
                    return dto;
                };
                var newRequestType = new RequestType
                {
                    Name = name,
                };
                await _requestTypeRepository.Insert(newRequestType);
                await _unitOfWork.SaveChangeAsync();
                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.CREATE_SUCCESS;
                dto.Data = new
                {
                    newRequestType.Id,
                    newRequestType.Name,
                };
                
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.message = ex.Message;
            }
            return dto;
        }

        public async Task<ResponseDTO> DeleteRequestTypeForAdmin(int id)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var requestType = await _requestTypeRepository.GetById(id);
                if (requestType == null)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    dto.Data = "Không tìm thấy loại yêu cầu.";
                    return dto;
                }
                await _requestTypeRepository.DeleteById(id);
                await _unitOfWork.SaveChangeAsync();
                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.DELETE_SUCCESS;
                dto.Data = $"Loại yêu cầu với ID {id} đã bị xóa thành công.";


            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.message = ex.Message;
            }
            return dto;
        }

        public async Task<ResponseDTO> GetAllRequestTypes()
        {
           ResponseDTO dto = new ResponseDTO();
            try
            {
                var requestTypes = await _requestTypeRepository.GetAllDataByExpression(null, 0, 0);
                dto.Data = requestTypes.Items.Select(rt => new
                {
                    rt.Id,
                    rt.Name,
                }).ToList();
                await _unitOfWork.SaveChangeAsync();
                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.Data = ex.Message;
            }
            return dto;
        }

        public async Task<ResponseDTO> GetAllRequestTypesForAdmin()
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var requestTypes = await _requestTypeRepository.GetAllDataByExpression(null , 0, 0); 
                dto.Data = requestTypes.Items.Select(rt => new
                {
                    rt.Id,
                    rt.Name
                }).ToList();

                await _unitOfWork.SaveChangeAsync(); 

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.Data = ex.Message;
            }
            return dto;
        }

        public async Task<ResponseDTO> UpdateRequestTypeForAdmin(int id, string name)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.INVALID_INPUT;
                    dto.Data = "Tên loại yêu cầu không được để trống.";
                    return dto;
                }
                var requestType = await _requestTypeRepository.GetById(id);
                if (requestType == null)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    dto.Data = "Không tìm thấy loại yêu cầu.";
                    return dto;
                };
                requestType.Name = name;
                await _requestTypeRepository.Update(requestType);
                await _unitOfWork.SaveChangeAsync();
                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.UPDATE_SUCESSFULLY;
                dto.Data = new
                {
                    requestType.Id,
                    requestType.Name,
                };
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.Data = ex.Message;
            }
            return dto;
        }
    }
}
