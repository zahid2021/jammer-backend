using Dapper;
using Jammer.DBContext;
using Jammer.OrderModule.DTOs;
using Jammer.OrderModule.Repositories.InterFace;
using System.Data;
using Jammer.OrderModule.Models;
using AutoMapper;
using Jammer.CartModule.DTOs;
using static Jammer.CartModule.DTOs.IntDTOs;

namespace Jammer.OrderModule.Repositories
{
    public class OrderRepositories : IOrderRepositories
    {
        readonly IDapperContext _context;

        public OrderRepositories(IDapperContext context)
        {

            _context = context;
        }

        public async Task<dynamic> CreateOrder(CreateOrderRequest request, string userid)
{
    using IDbConnection db = _context.CreateConnection();

   
        string fetchProductsQuery = @"SELECT Id, Price, CategoryId FROM Product WHERE Id IN @ProductIds;";
        var productPrices = await db.QueryAsync<ProductPrice>(fetchProductsQuery, new { ProductIds = request.Items.Select(i => i.ProductId).ToList() });

        if (!productPrices.Any())
            return "Invalid products in the request.";

        decimal totalAmount = 0;
        var productPricesDict = productPrices.ToDictionary(p => p.Id, p => p);

        foreach (var item in request.Items)
        {
            if (!productPricesDict.TryGetValue(item.ProductId, out var product))
                return $"Product with ID {item.ProductId} not found.";

            decimal finalPrice = product.Price;

            if (item.CouponId.HasValue && item.CouponId!=0)
            {
                finalPrice = await ApplyCouponIfApplicable( item.CouponId.Value, item.ProductId, product.Price, product.CategoryId);
            }

            totalAmount += finalPrice * item.Quantity;
        }


                string insertOrderQuery = @"
                 INSERT INTO Orders 
                     (UserId, TotalAmount,  City, Street, PostalCode, Region) 
                 VALUES 
                     (@UserId, @TotalAmount, @City, @Street, @PostalCode, @Region);
                 SELECT LAST_INSERT_ID();";
                OrderDTO model = new OrderDTO
                {
                    UserId = userid,
                    TotalAmount = totalAmount,
                    City = request. City,                
                    Street = request. Street,            
                    PostalCode = request.PostalCode,   
                    Region = request.Region
                };

                var orderId = await db.ExecuteScalarAsync<int>(insertOrderQuery, model);

        foreach (var item in request.Items)
        {
            if (!productPricesDict.TryGetValue(item.ProductId, out var product))
                continue; 

            decimal finalPrice = product.Price;

            if (item.CouponId.HasValue)
            {
                finalPrice = await ApplyCouponIfApplicable( item.CouponId.Value, item.ProductId, product.Price, product.CategoryId);
            }

            string insertOrderItemQuery = @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price)
                                            VALUES (@OrderId, @ProductId, @Quantity, @Price);";
            await db.ExecuteAsync(insertOrderItemQuery, new { OrderId = orderId, ProductId = item.ProductId, Quantity = item.Quantity, Price = finalPrice });
        }

        return "null";
   
}

        public async Task<bool> UpdateOrderStatus(int orderId, string status)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"UPDATE Orders SET Status = @Status WHERE Id = @OrderId;";
            int rowsAffected = await db.ExecuteAsync(query, new CreateOrder( status,  orderId ));


            if (rowsAffected == 0)
                return false;

            return true;


        }

        private async Task<decimal> ApplyCouponIfApplicable( int couponId, int productId, decimal originalPrice, int categoryId)
        {
            using IDbConnection db = _context.CreateConnection();

            GetCouponDTO? coupon;

            string couponQuery = @"SELECT * FROM Coupons WHERE Id = @Id AND ExpirationDate > NOW() LIMIT 1;";
            if (db != null && couponQuery != null)
            {
                coupon = await db.QueryFirstOrDefaultAsync<GetCouponDTO>(couponQuery, new IntDTOs(couponId));
            }
            else
            {
                return originalPrice;
            }
            string couponProductQuery = @"SELECT COUNT(*) FROM CouponProduct WHERE CouponId = @CouponId AND ProductId = @ProductId;";
            int isCouponApplicable = await db.ExecuteScalarAsync<int>(couponProductQuery, new CouponAndProductId(  couponId, productId ));
            couponProductQuery = @"SELECT COUNT(*) FROM couponcategory WHERE CouponId = @CouponId AND CategoryId = @ProductId;";
            int isCouponApplicable1 = await db.ExecuteScalarAsync<int>(couponProductQuery, new CouponAndProductId(couponId, categoryId));

            if (isCouponApplicable == 0 && isCouponApplicable1 == 0)
            {
                Console.WriteLine($"Coupon with ID {couponId} does not apply to product with ID {productId}.");
                return originalPrice;
            }

            decimal finalPrice = originalPrice;

            if (coupon.DiscountType == "PERCENTAGE")
            {
                finalPrice -= (originalPrice * (coupon.Discount / 100));
                Console.WriteLine($"Applying {coupon.Discount}% discount on product with ID {productId}. Original price: {originalPrice}, Final price: {finalPrice}");
            }
            else if (coupon.DiscountType == "FLAT")
            {
                if ((originalPrice - coupon.Discount) >= (originalPrice * 0.7m))
                {
                    finalPrice -= coupon.Discount;
                    Console.WriteLine($"Applying fixed discount of {coupon.Discount} on product with ID {productId}. Original price: {originalPrice}, Final price: {finalPrice}");
                }
                else
                {
                    Console.WriteLine($"Coupon discount exceeds the allowed limit for product with ID {productId}. No discount applied.");
                }
            }

            return finalPrice;
        }






        public async Task<OrderDTOS> GetOrderById(int orderId, IMapper mapper)
{
    using IDbConnection db = _context.CreateConnection();

    string orderQuery = @"
    SELECT 
        o.Id AS OrderId, 
    o.TotalAmount, 
    o.Status, 
    o.City,
    o.Street,
    o.PostalCode,
    o.Region,
    oi.Id AS OrderItemId,
    oi.ProductId,
    p.ProductURL,
    p.Name AS ProductName,
    pi.ImagePath AS ProductImagePath,
    oi.Quantity,
    oi.Price
    FROM 
        Orders o
    LEFT JOIN 
        OrderItems oi ON o.Id = oi.OrderId
    LEFT JOIN 
        Product p ON oi.ProductId = p.Id
    LEFT JOIN 
        ProductImage pi ON p.Id = pi.ProductId
    WHERE 
        o.Id = @OrderId;";

    var orderDtos = new List<OrderDTOS>();

    var order = await db.QueryAsync<OrderDTOS, OrderItemDTO, OrderDTOS>(
        orderQuery,
        (ord, item) =>
        {
            var existingOrder = orderDtos.SingleOrDefault(o => o.OrderId == ord.OrderId);
            if (existingOrder == null)
            {
                existingOrder = mapper.Map<OrderDTOS>(ord);
                existingOrder.Items = new List<OrderItemDTO>();
                orderDtos.Add(existingOrder);
            }

            if (item != null)
            {
                var existingItem = existingOrder.Items.SingleOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem == null)
                {
                    existingItem = mapper.Map<OrderItemDTO>(item);
                    existingOrder.Items.Add(existingItem);
                }

                if (!string.IsNullOrEmpty(item.ProductImagePath))
                {
                    existingItem.ProductImagePath = item.ProductImagePath;
                }
            }

            return existingOrder;
        },
        new { OrderId = orderId },
        splitOn: "OrderItemId, ProductName");

    return orderDtos.FirstOrDefault();
}



        public async Task<dynamic> GetOrdersByUserId(string userId)
        {
            using IDbConnection db = _context.CreateConnection();

            string ordersQuery = @"SELECT * FROM Orders WHERE UserId = @UserId;";
            var orders = await db.QueryAsync<Order>(ordersQuery, new { UserId = userId });

            if (!orders.Any())
                return "No orders found for this user.";

            var orderResponses = new List<OrderResponse>();

            foreach (var order in orders)
            {

                string orderItemsQuery = @"
                                            SELECT 
                                                oi.Id,
                                                oi.OrderId, p.ProductURL,
                                                oi.ProductId,
                                                p.Name AS ProductName,
                                                (SELECT pi.ImagePath FROM ProductImage pi WHERE pi.ProductId = p.Id LIMIT 1) AS ProductImagePath,
                                                oi.Quantity,
                                                oi.Price
                                            FROM 
                                                OrderItems oi
                                            LEFT JOIN 
                                                Product p ON oi.ProductId = p.Id
                                            WHERE 
                                                oi.OrderId = @OrderId;";

                var items = await db.QueryAsync<OrderItemDTO>(orderItemsQuery, new { OrderId = order.Id });

                orderResponses.Add(new OrderResponse
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    City=order.City,
                    Street= order.Street,
                    PostalCode = order.PostalCode,
                    Region = order.Region,

                    Items = items.ToList()
                    
                });
            }

            return orderResponses;
        }



        public async Task<dynamic> CancelOrder(int orderId)
        {

            using IDbConnection db = _context.CreateConnection();

            string query = @"UPDATE Orders SET Status = 'Canceled' WHERE Id = @OrderId;";
            int rowsAffected = await db.ExecuteAsync(query, new { OrderId = orderId });

            if (rowsAffected == 0)
                return "Order not found or already canceled.";

            return "Order has been canceled successfully.";


        }
        public async Task<dynamic> GetOrdersByStatus(string status)
        {

            using IDbConnection db = _context.CreateConnection();

            string query = @"SELECT * FROM Orders WHERE Status = @Status;";
            var orders = await db.QueryAsync<Order>(query, new { Status = status });

            if (!orders.Any())
                return "No orders found with the specified status.";

            var orderResponses = new List<OrderResponse>();

            foreach (var order in orders)
            {
                string orderItemsQuery = @"SELECT * FROM OrderItems WHERE OrderId = @OrderId;";
                var items = await db.QueryAsync<OrderItemDTO>(orderItemsQuery, new { OrderId = order.Id });

                orderResponses.Add(new OrderResponse
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    Items = items.ToList()
                });
            }

            return orderResponses;


        }
        public async Task<List<OrderUserIDDTOS>> GetAllOrders(IMapper mapper)
        {
            using IDbConnection db = _context.CreateConnection();

           
            string orderQuery = @"
                                    SELECT 
                                        o.Id AS OrderId, 
                                        o.TotalAmount, 
                                        o.Status, 
                                        o.City,
                                        o.Street,
                                        o.PostalCode,
                                        o.Region,
                                        o.UserId, 
                                        o.OrderDate
                                    FROM 
                                        Orders o;";

            var orderDtos =( await db.QueryAsync<OrderUserIDDTOS>(orderQuery)).ToList();

            foreach (var order in orderDtos)
            {
                string orderItemQuery = @"
                                            SELECT 
                                                oi.Id AS OrderItemId,
                                                oi.ProductId,
                                                p.ProductURL,
                                                p.Name AS ProductName,
                                                pi.ImagePath AS ProductImagePath,
                                                oi.Quantity,
                                                oi.Price
                                            FROM 
                                                OrderItems oi
                                            LEFT JOIN 
                                                Product p ON oi.ProductId = p.Id
                                            LEFT JOIN 
                                                ProductImage pi ON p.Id = pi.ProductId
                                            WHERE 
                                                oi.OrderId = @OrderId;";

                var orderItems = await db.QueryAsync<OrderItemDTO>(orderItemQuery, new { OrderId = order.OrderId });

                order.Items = orderItems.ToList();
            }

            // Get user names for each order
            var userIds = orderDtos.Select(o => o.UserId).Distinct().ToList();
            string userQuery = "SELECT Id, fullname FROM User WHERE Id IN @UserIds;";
            var users = await db.QueryAsync<UserDTO>(userQuery, new { UserIds = userIds });

            var userDictionary = users.ToDictionary(u => u.Id, u => u.fullname);

            foreach (var order in orderDtos)
            {
                if (userDictionary.TryGetValue(order.UserId, out var userName))
                {
                    order.fullname = userName;
                }
            }

            return orderDtos;
        }


        public async Task<dynamic> DeleteOrder(int orderId)
        {

            using IDbConnection db = _context.CreateConnection();

            string deleteOrderItemsQuery = @"DELETE FROM OrderItems WHERE OrderId = @OrderId;";
            await db.ExecuteAsync(deleteOrderItemsQuery, new { OrderId = orderId });
            string deleteOrderQuery = @"DELETE FROM Orders WHERE Id = @OrderId;";
            int rowsAffected = await db.ExecuteAsync(deleteOrderQuery, new { OrderId = orderId });

            if (rowsAffected == 0)
                return "Order not found.";

            return "Order deleted successfully.";


        }
        public async Task<OrderSummary> GetOrderSummary()
        {

            using IDbConnection db = _context.CreateConnection();

            string query = @"SELECT COUNT(*) AS TotalOrders, SUM(TotalAmount) AS TotalSales FROM Orders;";
            var summary = await db.QueryFirstOrDefaultAsync<OrderSummary>(query);

            return summary;


        }
        public async Task<MonthlyOrderReport> GetMonthlyOrderReport()
        {

            using IDbConnection db = _context.CreateConnection();

            string query = @"
                SELECT 
                    YEAR(OrderDate) AS Year, 
                    MONTH(OrderDate) AS Month, 
                    COUNT(*) AS TotalOrders, 
                    SUM(TotalAmount) AS TotalSales
                FROM Orders
                GROUP BY YEAR(OrderDate), MONTH(OrderDate)
                ORDER BY Year, Month;";

            var monthlyReport = await db.QueryFirstOrDefaultAsync<MonthlyOrderReport>(query);

            return monthlyReport;


        }
        public async Task<OverallOrderReport> GetOverallOrderReport()
        {

            using IDbConnection db = _context.CreateConnection();

            string query = @"
                        SELECT 
                            COUNT(*) AS TotalOrders, 
                            SUM(TotalAmount) AS TotalSales,
                            AVG(TotalAmount) AS AverageOrderValue,
                            MIN(TotalAmount) AS MinimumOrderValue,
                            MAX(TotalAmount) AS MaximumOrderValue
                        FROM Orders;";

            var overallReport = await db.QueryFirstOrDefaultAsync<OverallOrderReport>(query);

            return overallReport;


        }
        public async Task<OrderStatusCount> GetOrderStatusCounts()
        {

            using IDbConnection db = _context.CreateConnection();

            string query = @"
                        SELECT 
                            Status,
                            COUNT(*) AS TotalCount
                        FROM Orders
                        GROUP BY Status;";

            var orderStatusCounts = await db.QueryFirstOrDefaultAsync<OrderStatusCount>(query);

            return orderStatusCounts;


        }



    }
}
