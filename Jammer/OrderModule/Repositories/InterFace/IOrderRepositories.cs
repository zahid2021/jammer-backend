using AutoMapper;
using Jammer.OrderModule.DTOs;

namespace Jammer.OrderModule.Repositories.InterFace
{
    public interface IOrderRepositories
    {
        Task<dynamic> CreateOrder(CreateOrderRequest request, string userid);
         Task<OrderDTOS> GetOrderById(int orderId, IMapper mapper);
        Task<bool> UpdateOrderStatus(int orderId, string status);
        Task<dynamic> GetOrdersByUserId(string userId);
        Task<dynamic> CancelOrder(int orderId);
        Task<dynamic> GetOrdersByStatus(string status);
        Task<dynamic> DeleteOrder(int orderId);
        Task<OrderSummary> GetOrderSummary();
        Task<MonthlyOrderReport> GetMonthlyOrderReport();
        Task<OverallOrderReport> GetOverallOrderReport();
        Task<OrderStatusCount> GetOrderStatusCounts();
        Task<List<OrderUserIDDTOS>> GetAllOrders(IMapper mapper);
    }
}
