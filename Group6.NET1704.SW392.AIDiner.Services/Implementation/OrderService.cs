﻿using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.BusinessCode;
using Group6.NET1704.SW392.AIDiner.DAL.Contract;
using Group6.NET1704.SW392.AIDiner.DAL.Models;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Services.Implementation
{
    public class OrderService : IOrderService
    {

        private IGenericRepository<Order> _orderRepository;
        private IUnitOfWork _unitOfWork;

        public OrderService(IGenericRepository<Order> orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO> CreateOrder(CreateOrderDTO request)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                if (request == null || request.CustomerId <= 0 || request.TableId <= 0 || request.TotalAmount <=0)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode=BusinessCode.INVALID_INPUT;
                    return dto;
                }
                Order newOrder = new Order  
                {
                    //CustomerId = request.CustomerId,
                    TableId = request.TableId,
                    TotalAmount = request.TotalAmount,
                    PaymentStatus = request.PaymentStatus,
                    CreatedAt = DateTime.Now,
                    Status = request.Status,
                };

                // Lưu vào database
                await _orderRepository.Insert(newOrder);
                await _unitOfWork.SaveChangeAsync();


                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.CREATE_ORDER_SUCCESSFULLY;
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode=BusinessCode.EXCEPTION;
            }
            return dto;
        }

        public async Task<ResponseDTO> CreateOrderByTable(CreateOrderByTableDTO request)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                if (request == null || request.TableId <= 0)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.INVALID_INPUT;
                    dto.message = "Invalid table ID";
                    return dto;
                }

                Order newOrder = new Order
                {
                    TableId = request.TableId,
                    TotalAmount = 0,
                    //PaymentStatus = 0, // Default payment status
                    CreatedAt = DateTime.UtcNow,
                    Status = "pending"
                };

                await _orderRepository.Insert(newOrder);
                await _unitOfWork.SaveChangeAsync();

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.CREATE_SUCCESS;
                dto.message = "Order created successfully";
                dto.Data = new OrderByTableDTO
                {
                    OrderId = newOrder.Id
                };
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.message = "Error" + ex.Message;
            }
            return dto;
        }

        public async Task<ResponseDTO> GetAllOrder()
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var orders = await _orderRepository.GetAllDataByExpression(null, 0, 0, null, true,
                     includes: new Expression<Func<Order, object>>[] { b => b.Table });
                dto.Data = orders.Items.Select(x => new OrderDTO
                {
                    Id = x.Id,
                    //CustomerId = x.CustomerId,
                    TableId = x.TableId,
                    TotalAmount = x.TotalAmount,
                    PaymentStatus = x.PaymentStatus,
                    CreatedAt = x.CreatedAt,
                    Status = x.Status,
                }).ToList();
                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;

            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
            }
            return dto;
        }

        public async Task<ResponseDTO> GetByOrderId(int id)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var order = await _orderRepository.GetAllDataByExpression(
                    filter: o => o.Id == id,0, 0,
                    includes: new Expression<Func<Order, object>>[]
                    {
                o => o.Table });

                var orderData = order.Items.FirstOrDefault();
                if (orderData == null)
                {
                    dto.IsSucess = false;
                    return dto;
                }

                dto.Data = new OrderDTO
                {
                    Id = orderData.Id,
                    TableId = orderData.TableId,
                    TableName = orderData.Table.Name,

                    TotalAmount = orderData.TotalAmount,
                    PaymentStatus = orderData.PaymentStatus,
                    CreatedAt = orderData.CreatedAt,
                    Status = orderData.Status,



                    //    TableName = new TableForOrderDTO
                    //    {
                    //        TableName = orderData.Table.Name,

                    //    }
                };

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
                dto.message = "Get Order by ID successfully";
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
            }
            return dto;
        }



    }
}

