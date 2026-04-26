using CarWash.Backend.DTOs.Order;

namespace CarWash.Backend.Services.Interfaces;

public interface IOrderService
{
    Task<List<OrderResponse>> GetMyOrdersAsync(int userId, GetMyOrdersRequest request);
}
