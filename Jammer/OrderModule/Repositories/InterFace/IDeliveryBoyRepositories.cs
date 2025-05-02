using Jammer.OrderModule.DTOs;

namespace Jammer.OrderModule.Repositories.InterFace
{
    public interface IDeliveryBoyRepositories
    {
        Task<List<DeliveryBoyDTO>> GetAllDeliveryBoys();
        Task<bool> AssignDelivery(string deliveryBoyId, int orderId);
        Task<List<DeliveryOrderDTO>> GetAssignedOrders(string deliveryBoyId);
        Task<DeliveryOrderDTO> GetOrderDetails(int orderId);
        Task<bool> UpdateOrderStatus(int orderId, string status);
        Task<bool> MarkOrderAsDelivered(int orderId);
    }

}
