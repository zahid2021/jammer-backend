using Jammer.DBContext;
using Jammer.OrderModule.DTOs;
using Jammer.OrderModule.Repositories.InterFace;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace Jammer.OrderModule.Repositories
{
    public class DeliveryBoyRepositories : IDeliveryBoyRepositories
    {
        readonly IDapperContext _context;

        public DeliveryBoyRepositories(IDapperContext context)
        {
            _context = context;
        }

        public async Task<bool> AssignDelivery(string deliveryBoyId, int orderId)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
                INSERT INTO assigndelivery (OrderId, DeliveryBoyId, AssignedAt, Status)
                VALUES (@OrderId, @DeliveryBoyId, @AssignedAt, 'Assigned');";

            int rowsAffected = await db.ExecuteAsync(query, new
            {
                DeliveryBoyId = deliveryBoyId,
                OrderId = orderId,
                AssignedAt = DateTime.UtcNow 
            });

            return rowsAffected > 0;
        }

        public async Task<List<DeliveryBoyDTO>> GetAllDeliveryBoys()
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
        SELECT Id, FullName, Email, PhoneNumber, Image
        FROM User
        WHERE RoleId = 4;"; // Assuming 4 is the RoleId for delivery boys

            var deliveryBoys = (await db.QueryAsync<DeliveryBoyDTO>(query)).ToList();

            return deliveryBoys;
        }


        public async Task<List<DeliveryOrderDTO>> GetAssignedOrders(string deliveryBoyId)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
                SELECT o.Id AS OrderId, o.TotalAmount, o.Status, o.City, o.Street, o.PostalCode, o.Region, 
                       u.FullName 
                FROM Orders o
                JOIN User u ON o.UserId = u.Id
                JOIN assigndelivery ad ON o.Id = ad.OrderId
                WHERE ad.DeliveryBoyId = @DeliveryBoyId;";

            var orders =( await db.QueryAsync<DeliveryOrderDTO>(query, new { DeliveryBoyId = deliveryBoyId })).ToList();

            return orders;
        }

        public async Task<DeliveryOrderDTO> GetOrderDetails(int orderId)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
                SELECT o.Id AS OrderId, o.TotalAmount, o.Status, o.City, o.Street, o.PostalCode, o.Region, 
                       u.FullName 
                FROM Orders o
                JOIN User u ON o.UserId = u.Id
                JOIN assigndelivery ad ON o.Id = ad.OrderId
                WHERE o.Id = @OrderId;";

            var order = await db.QueryFirstOrDefaultAsync<DeliveryOrderDTO>(query, new { OrderId = orderId });

            return order;
        }

        public async Task<bool> UpdateOrderStatus(int orderId, string status)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
                UPDATE assigndelivery 
                SET Status = @Status 
                WHERE OrderId = @OrderId;";

            int rowsAffected = await db.ExecuteAsync(query, new { Status = status, OrderId = orderId });
            query = "";
            query = @"
                UPDATE orders 
                SET Status = @Status 
                WHERE Id = @OrderId;";

            int rowsAffected1 = await db.ExecuteAsync(query, new { Status = status, OrderId = orderId });

            return rowsAffected > 0 && rowsAffected > 0;
        }

        public async Task<bool> MarkOrderAsDelivered(int orderId)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
                UPDATE assigndelivery 
                SET Status = 'Delivered' 
                WHERE OrderId = @OrderId;";
            int rowsAffected = await db.ExecuteAsync(query, new { OrderId = orderId });
            query = "";
            query = @"
                UPDATE orders 
                SET Status = 'Delivered'
                WHERE Id = @OrderId;";

            int rowsAffected1 = await db.ExecuteAsync(query, new {  OrderId = orderId });

            return rowsAffected > 0 && rowsAffected > 0;

         
        }
    }
}
