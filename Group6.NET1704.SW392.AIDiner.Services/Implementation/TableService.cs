using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.AdminDTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.BusinessCode;
using Group6.NET1704.SW392.AIDiner.Common.DTO.Request;
using Group6.NET1704.SW392.AIDiner.DAL.Contract;
using Group6.NET1704.SW392.AIDiner.DAL.Models;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Implementation
{
    public class TableService : ITableService
    {
        private IGenericRepository<Table> _tableRepository;
        private IGenericRepository<Restaurant> _restaurantRepository;

        private IUnitOfWork _unitOfWork;

        public TableService(IGenericRepository<Table> tableRepository, IGenericRepository<Restaurant> restaurantRepository, IUnitOfWork unitOfWork)
        {
            _tableRepository = tableRepository;
            _restaurantRepository = restaurantRepository;
            _unitOfWork = unitOfWork;
        }





        //public async Task<ResponseDTO> GetAllTable()
        //{
        //    ResponseDTO dto = new ResponseDTO();
        //    try
        //    {
        //        var tables = await _tableRepository.GetAllDataByExpression(null, 0, 0, null, true);
        //        dto.Data = tables.Items.Select(d => new TableDTO
        //        {
        //            Id = d.Id,
        //            Name = d.Name,
        //            Description = d.Description,
        //            //Qrcode = d.Qrcode,
        //            CreatedAt = d.CreatedAt,
        //            Status = d.Status,
        //        }).ToList();
        //        dto.IsSucess = true;
        //        dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
        //    }
        //    catch (Exception ex)
        //    {
        //        dto.IsSucess = false;
        //        dto.BusinessCode = BusinessCode.EXCEPTION;
        //    }
        //    return dto;
        //}

        public async Task<ResponseDTO> GetAllTableForAdmin()
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var tables = await _tableRepository.GetAllDataByExpression(
                    filter: null,
                    pageNumber: 0,
                    pageSize: 0,
                    orderBy: null,
                    isAscending: true,
                    includes: t => t.Restaurant 
                );

                dto.Data = tables.Items.Select(d => new TableAdminDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    CreatedAt = d.CreatedAt,
                    IsDeleted = d.IsDeleted,
                    RestaurantId = d.RestaurantId,
                    RestaurantName = d.Restaurant != null ? d.Restaurant.Name : "Unknown",
                    RestaurantImage = d.Restaurant != null ? d.Restaurant.Image : ""
                }).ToList();

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
                dto.message = "Get all table for admin successfully";
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.message = ex.Message;
            }
            return dto;
        }

        public async Task<ResponseDTO> CreateTableForAdmin(CreateTableForAdminDTO createTableDTO)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                if (createTableDTO == null || string.IsNullOrWhiteSpace(createTableDTO.Name) || createTableDTO.RestaurantId <= 0)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.INVALID_INPUT;
                    dto.message = "Invalid table data.";
                    return dto;
                }

                // Kiểm tra xem nhà hàng có tồn tại không
                var restaurant = await _restaurantRepository.GetById(createTableDTO.RestaurantId);
                if (restaurant == null)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    dto.message = "Restaurant not found.";
                    return dto;
                }

                // Tạo đối tượng Table từ DTO
                var newTable = new Table
                {
                    Name = createTableDTO.Name,
                    Description = createTableDTO.Description,
                    RestaurantId = createTableDTO.RestaurantId,
                    CreatedAt = DateTime.UtcNow,  
                    IsDeleted = false,
                    Status = "available"
                };

                var createdTable = await _tableRepository.Insert(newTable);

                dto.Data = new TableAdminDTO
                {
                    Id = createdTable.Id,
                    Name = createdTable.Name,
                    Description = createdTable.Description,
                    CreatedAt = createdTable.CreatedAt,
                    IsDeleted = createdTable.IsDeleted,
                    RestaurantId = createdTable.RestaurantId,
                    RestaurantName = restaurant.Name,   
                    RestaurantImage = restaurant.Image 
                };

                await _unitOfWork.SaveChangeAsync();

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.CREATE_SUCCESS;
                dto.message = "Table created successfully.";
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.message = ex.Message;
            }

            return dto;
        }

        public async Task<ResponseDTO> UpdateTableForAdmin(int id, UpdateTableRequest updateRequest)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var table = await _tableRepository.GetQueryable()
                    .Include(t => t.Restaurant)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (table == null)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    dto.Data = "Table not found";
                    return dto;
                }

                // Cập nhật thông tin nếu có thay đổi
                if (!string.IsNullOrEmpty(updateRequest.Name)) table.Name = updateRequest.Name;
                if (!string.IsNullOrEmpty(updateRequest.Description)) table.Description = updateRequest.Description;
                if (updateRequest.RestaurantId.HasValue) table.RestaurantId = updateRequest.RestaurantId.Value;
                if (updateRequest.IsDeleted.HasValue) table.IsDeleted = updateRequest.IsDeleted.Value;

                await _tableRepository.Update(table);
                await _unitOfWork.SaveChangeAsync();

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.UPDATE_SUCESSFULLY;
                dto.Data = new
                {
                    table.Id,
                    table.Name,
                    table.Description,
                    CreatedAt = table.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
                    table.IsDeleted,
                    table.RestaurantId,
                    RestaurantName = table.Restaurant?.Name,
                    RestaurantImage = table.Restaurant?.Image
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

        public async Task<ResponseIsSucessDTO> DeleteTableForAdmin(int id)
        {
            ResponseIsSucessDTO dto = new ResponseIsSucessDTO();
            try
            {
                var table = await _tableRepository.GetById(id);
                if (table == null)
                {
                    dto.IsSucess = false;

                    return dto;
                }
                table.IsDeleted = true;
                await _tableRepository.Update(table);
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

