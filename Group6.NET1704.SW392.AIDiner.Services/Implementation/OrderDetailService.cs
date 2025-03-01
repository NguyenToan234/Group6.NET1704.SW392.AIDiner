using Group6.NET1704.SW392.AIDiner.Common.DTO;
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
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IGenericRepository<OrderDetail> _orderDetailRepositoy;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<Dish> _dishRepository;

        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailService(IGenericRepository<OrderDetail> orderDetailRepositoy, IGenericRepository<Order> orderRepository, IGenericRepository<Dish> dishRepository, IUnitOfWork unitOfWork)
        {
            _orderDetailRepositoy = orderDetailRepositoy;
            _orderRepository = orderRepository;
            _dishRepository = dishRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO> GetOrderDetailByOrderID(int orderId)
        {
            ResponseDTO dto = new ResponseDTO();

            try
            {
                if (orderId <= 0)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.INVALID_INPUT;
                    return dto;
                }

                // Lấy thông tin Order từ repository
                var order = await _orderRepository.GetByExpression(o => o.Id == orderId);

                if (order == null)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    return dto;
                }

                // Lấy danh sách OrderDetail theo OrderId, kèm thông tin món ăn từ Dish
                var orderDetails = await _orderDetailRepositoy.GetAllDataByExpression(
                    od => od.OrderId == orderId, 0, 0, includes: new Expression<Func<OrderDetail, object>>[]
                    {
                od => od.Dish  // Include Dish để lấy thông tin chi tiết món ăn
                    }
                );

                if (orderDetails == null || orderDetails.Items.Count == 0)
                {
                    dto.IsSucess = false;
                    dto.BusinessCode = BusinessCode.NOT_FOUND;
                    return dto;
                }

                // Gộp thông tin Order và danh sách OrderDetail vào response
                dto.Data = new
                {
                   
                        OrderDetails = orderDetails.Items.Select(orderdetail => new
                        {
                            orderdetail.Id,
                            Name = orderdetail.Dish.Name, // Lấy tên món ăn từ Dish
                            Image = orderdetail.Dish.Image, // Lấy ảnh món ăn từ Dish
                            orderdetail.Quantity,
                            orderdetail.Price,
                            orderdetail.Status,
                            orderdetail.Note,
                        }).ToList()
                    
                };

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.GET_DATA_SUCCESSFULLY;
                dto.message = "Get OrderID by Orderdetail Successfully";
            }
            catch (Exception ex)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
            }

            return dto;
        }


        public async Task<ResponseDTO> AddDishToOrder(int orderId, List<DishRequestDTO> dishes)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var order = await _orderRepository.GetByExpression(o => o.Id == orderId);
                if (order == null)
                {
                    return new ResponseDTO { IsSucess = false, BusinessCode = BusinessCode.NOT_FOUND };
                }

                if (dishes == null)
                {
                    return new ResponseDTO { IsSucess = false, BusinessCode = BusinessCode.INVALID_INPUT };
                }

                var orderDetails = new List<OrderDetail>();
                decimal totalAmount = order.TotalAmount;

                foreach (var dish in dishes)
                {
                    var menuDish = await _dishRepository.GetByExpression(d => d.Id == dish.DishId);
                    if (menuDish == null)
                    {
                        return new ResponseDTO { IsSucess = false, BusinessCode = BusinessCode.NOT_FOUND };
                    }

                    var existingOrderDetails = await _orderDetailRepositoy.GetAllDataByExpression(
                        od => od.OrderId == orderId && od.DishId == dish.DishId, 0, 0);

                    var existingOrderDetail = existingOrderDetails?.Items?.FirstOrDefault();

                    if (existingOrderDetail != null)
                    {
                        existingOrderDetail.Quantity += dish.Quantity;
                        existingOrderDetail.Price = existingOrderDetail.Quantity * menuDish.Price;
                        totalAmount += dish.Quantity * menuDish.Price;
                    }
                    else
                    {
                        var orderDetail = new OrderDetail
                        {
                            OrderId = orderId,
                            DishId = dish.DishId,
                            Quantity = dish.Quantity,
                            Price = menuDish.Price * dish.Quantity,
                            Status = "pending"
                        };
                        orderDetails.Add(orderDetail);
                        totalAmount += orderDetail.Price;
                    }
                }

                if (orderDetails.Count > 0)
                {
                    await _orderDetailRepositoy.InsertRange(orderDetails);
                }

                order.TotalAmount = totalAmount;
                await _unitOfWork.SaveChangeAsync();

                dto.IsSucess = true;
                dto.BusinessCode = BusinessCode.CREATE_ORDER_SUCCESSFULLY;
                dto.Data = new
                {
                    OrderId = orderId,
                    OrderDetails = orderDetails.Select(od => new
                    {
                        DishId = od.DishId,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        Status = od.Status
                    }).ToList()
                };
            }
            catch (Exception)
            {
                dto.IsSucess = false;
                dto.BusinessCode = BusinessCode.EXCEPTION;
            }

            return dto;
        }
    }
}