using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.Common.DTO.BusinessCode;
using Group6.NET1704.SW392.AIDiner.DAL.Contract;
using Group6.NET1704.SW392.AIDiner.DAL.Models;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Microsoft.EntityFrameworkCore;
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
        private IGenericRepository<OrderDetail> _orderDetailRepository;

        private IUnitOfWork _unitOfWork;

        public OrderService(IGenericRepository<Order> orderRepository, IGenericRepository<OrderDetail> orderDetailRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO> CreateOrder(CreateOrderDTO request)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                if (request == null || request.CustomerId <= 0 || request.TableId <= 0 || request.TotalAmount <= 0)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.INVALID_INPUT;
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
                dto.BusinessCode = BusinessCode.EXCEPTION;
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
                filter: o => o.Id == id, 0, 0,
                includes: new Expression<Func<Order, object>>[]
                {
                    o => o.Table,
                    o => o.OrderDetails
                });

                var orderData = order.Items.FirstOrDefault();

                if (orderData != null && orderData.OrderDetails != null)
                {
                    foreach (var detail in orderData.OrderDetails)
                    {
                        detail.Dish = await _dishRepository.GetById(detail.DishId);
                    }
                }

                var dishes = orderData.OrderDetails != null && orderData.OrderDetails.Any()
                    ? orderData.OrderDetails
                        .Select(d => new Order_OrderDetailDTO
                        {
                            Id = d.Id,
                            Name = d.Dish.Name,
                            Image = d.Dish.Image,
                            Price = d.Price,
                            Quantity = d.Quantity,
                            Status = d.Status
                        }).ToList()
                    : new List<Order_OrderDetailDTO>();

                dto.Data = new OrderDTO
                {
                    Id = orderData.Id,
                    TableId = orderData.TableId,
                    TableName = orderData.Table?.Name ?? "Unknown Table",
                    TotalAmount = orderData.TotalAmount,
                    PaymentStatus = orderData.PaymentStatus,
                    Status = orderData.Status,
                    Dishes = dishes
                };

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
                dto.message = "Get Detail Order Successfully.";
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.message = "Error: " + ex.Message;
            }
            return dto;
        }


        public async Task<ResponseDTO> UpdateOrderByAdmin(int orderId, UpdateOrderDTO request)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var order = await _orderRepository.GetQueryable()
                    .Include(o => o.Table)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    dto.message = "Order not found";
                    return dto;
                }

                // Cập nhật thông tin cơ bản của đơn hàng
                order.TableId = request.TableId;
                order.PaymentStatus = request.PaymentStatus;
                order.Status = request.Status;

                await _orderDetailRepository.DeleteWhere(od => od.OrderId == orderId);

                // Thêm orderDetail mới
                List<OrderDetail> newOrderDetails = new List<OrderDetail>();
                foreach (var dish in request.Dishes)
                {
                    newOrderDetails.Add(new OrderDetail
                    {
                        OrderId = orderId,
                        DishId = dish.DishId,
                        Quantity = dish.Quantity,
                        Price = dish.Price,
                        Status = dish.Status
                    });
                }
                await _orderDetailRepository.InsertRange(newOrderDetails);
                order.TotalAmount = newOrderDetails.Sum(d => d.Price * d.Quantity);
                await _unitOfWork.SaveChangeAsync();
                // Lưu thay đổi vào database
                await _unitOfWork.SaveChangeAsync();

                var updatedOrder = new UpdateOrderResponseDTO
                {
                    Id = order.Id,
                    TableId = order.TableId.ToString(),
                    TableName = order.Table != null ? order.Table.Name : "Unknown Table",
                    TotalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity),
                    PaymentStatus = order.PaymentStatus,
                    Status = order.Status,
                    Dishes = newOrderDetails.Select(d => new OrderDetailResponseDTO
                    {
                        Id = d.Id, // Đảm bảo lấy đúng id của OrderDetail
                        Name = d.Dish != null ? d.Dish.Name : "Unknown Dish",
                        Image = d.Dish != null ? d.Dish.Image : "No Image",
                        Price = d.Price,
                        Quantity = d.Quantity,
                        Status = d.Status
                    }).ToList()
                };

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.UPDATE_SUCESSFULLY;
                dto.message = "Order updated successfully";
                dto.Data = updatedOrder;

            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
                dto.message = "Error: " + ex.Message;
            }
            return dto;
        }

    }
}

