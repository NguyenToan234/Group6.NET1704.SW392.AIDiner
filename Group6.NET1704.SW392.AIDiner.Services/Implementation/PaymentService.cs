﻿using Group6.NET1704.SW392.AIDiner.Common.DTO.BusinessCode;
using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.DAL.Contract;
using Group6.NET1704.SW392.AIDiner.DAL.Models;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Group6.NET1704.SW392.AIDiner.Common.Request;
using Group6.NET1704.SW392.AIDiner.Services.Util;
using Group6.NET1704.SW392.AIDiner.Services.Enums;
using Microsoft.AspNetCore.SignalR;
using Group6.NET1704.SW392.AIDiner.Services.Hubs;

namespace Group6.NET1704.SW392.AIDiner.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<PaymentMethod> _paymentMethodRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<OrderDetailHub> _orderHub;

        public PaymentService(
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<PaymentMethod> paymentMethodRepository,
            IGenericRepository<Order> orderRepository,
            IUnitOfWork unitOfWork,
            IHubContext<OrderDetailHub> orderHub)
        {
            _paymentRepository = paymentRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderHub = orderHub;
        }

        public async Task<ResponseDTO> GetPayments(int? restaurantId)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var payments = await _paymentRepository.GetAllDataByExpression(
                    p => restaurantId == null || p.Order.Table.RestaurantId == restaurantId,
                    0, 0, null, true, p => p.Method, p => p.Order.Table);

                dto.Data = payments.Items.Select(p => new PaymentDTO
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    MethodId = p.MethodId,
                    MethodName = p.Method.Name,
                    TransactionCode = p.TransactionCode,
                    CreatedAt = p.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
                    Amount = p.Amount,
                    Description = p.Description,
                    Status = p.Status
                }).ToList();

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
                dto.message = "Get Payment from restaurant successfully";

            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.Data = ex.Message;
            }
            return dto;
        }

        public async Task<ResponseDTO> GetPaymentByRestaurantId(int? restaurantId)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var payments = await _paymentRepository.GetAllDataByExpression(
                    p => !restaurantId.HasValue || p.Order.Table.RestaurantId == restaurantId.Value,
                    0, 0, null, true, p => p.Method, p => p.Order.Table);

                dto.Data = payments.Items.Select(p => new PaymentDTO
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    MethodId = p.MethodId,
                    MethodName = p.Method?.Name ?? "Unknown",
                    TransactionCode = p.TransactionCode,
                    CreatedAt = p.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
                    Amount = p.Amount,
                    Description = p.Description,
                    Status = p.Status
                }).ToList();

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
                dto.message = "Get payments successfully";
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.Data = ex.Message;
            }
            return dto;
        }

        public async Task<ResponseDTO> PaidByCash(PaidByCashRequest request)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var order = await _unitOfWork.Orders.GetByExpression(o => o.Id == request.orderId, o => o.Table);

                if(order == null || order.Status.Equals("completed"))
                {
                    throw new Exception("Đơn hàng đã được thanh toán hoặc không tồn tại");
                }

                var table = order.Table;

                Payment newPayment = new Payment
                {
                    OrderId = request.orderId,
                    MethodId = 2,
                    TransactionCode = "",
                    CreatedAt = TimeZoneUtil.GetCurrentTime(),
                    Amount = order.TotalAmount,
                    Description = request.description != null ? request.description : "Thanh toán cho hóa đơn " + order.Id,
                    Status = true
                };
                
                await _unitOfWork.Payments.Insert(newPayment);

                // complete order , free talbe
                order.Status = OrderStatus.completed.ToString();
                order.PaymentStatus = true;
                table.Status = TableStatus.available.ToString();

                await _unitOfWork.SaveChangeAsync();

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
                dto.message = "Paid for order successfully";

                await _orderHub.Clients.Group("Order" + request.orderId.ToString()).SendAsync("PaidComplete", newPayment.Id);
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.Data = ex.Message;
            }           
            return dto;
        }

        public async Task<ResponseDTO> GetPaymentInfoById(int paymentId)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var payment = await _paymentRepository.GetByExpression(
                    p => p.Id == paymentId,
                    p => p.Method,
                    p => p.Order.OrderDetails);

                if (payment == null)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    dto.message = "Payment not found";
                    return dto;
                }

                dto.Data = new 
                {
                    Id = payment.Id,
                    OrderId = payment.OrderId,
                    MethodId = payment.Method.Name,
                    MethodName = payment.Method?.Name ?? "Unknown",
                    TransactionCode = payment.TransactionCode,
                    CreatedAt = payment.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
                    Amount = payment.Amount,
                    Description = payment.Description,
                    Status = payment.Status
                };

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
                dto.message = "Get payment successfully";
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
