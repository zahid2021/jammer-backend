using AutoMapper;
using Jammer.DBContext;
using Microsoft.AspNetCore.Mvc;
using Jammer.OrderModule.DTOs;
using Microsoft.AspNetCore.Authorization;
using Jammer.UserModule.DTOs;
using Jammer.Utills;

using Jammer.OrderModule.Repositories.InterFace;
using Jammer.ResponseMessage;

namespace Jammer.OrderModule.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        private readonly IDapperContext _context;
        private readonly IMapper _mapper;
        IOrderRepositories _orderRepositories;

        public OrderController(IDapperContext context, IMapper mapper, IOrderRepositories orderRepositories)
        {
            _context = context;
            _mapper = mapper;
            _orderRepositories = orderRepositories;

        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userId = Request.GetUser();
            ResponseDTO response = new();
            try
            {
                var data = await _orderRepositories.CreateOrder(request, userId);

                response.Data = data;
                response.Message = data == null|| data== "null" ? MessageDisplay.Orderadd :MessageDisplay.notFound  ;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }




        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
                ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {

            try
            {
                var data = await _orderRepositories.GetOrderById(orderId,_mapper);

                response.Data = data;
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
                ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
            try
            {
                if (await _orderRepositories.UpdateOrderStatus(orderId, status))
                {
                    response.Message = MessageDisplay.Orderupdate;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Orderupdateerror;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetOrdersByUserId()
        {

            var userId = Request.GetUser();
            ResponseDTO response = new();
            try
            {
                var data = await _orderRepositories.GetOrdersByUserId(userId);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderget;
                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpPut("{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            ResponseDTO response = new();
            try
            {
                var data = await _orderRepositories.CancelOrder(orderId);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderupdate;
                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpGet("{status}")]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
                ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
            try
            {
                var data = await _orderRepositories.GetOrdersByStatus(status);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderupdate;
                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            ResponseDTO response = new();
            try
            {
                var data = await _orderRepositories.DeleteOrder(orderId);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderdelete;
                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetOrderSummary()
        {

            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                var data = await _orderRepositories.GetOrderSummary();

                response.Data = data;
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }


        [HttpGet("monthly-order-report")]
        public async Task<IActionResult> GetMonthlyOrderReport()
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                var data = await _orderRepositories.GetMonthlyOrderReport();

                response.Data = data;
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }
        [HttpGet("overall-order-report")]
        public async Task<IActionResult> GetOverallOrderReport()
        {
            ResponseDTO response = new();

            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                var data = await _orderRepositories.GetOverallOrderReport();

                response.Data = data;
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }



        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            ResponseDTO response = new();

            var role = Request.GetRole();
            if (role == "Admin")
            {
                    try
                    {
                        var data = await _orderRepositories.GetAllOrders(_mapper);
                        response.Data = data;
                        response.Message = data == null || !data.Any() ? MessageDisplay.notFound : MessageDisplay.Orderget;
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        response.Message = MessageDisplay.error;
                        return BadRequest(response);
                    }
                }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetOrderStatusCounts()
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                var data = await _orderRepositories.GetOrderStatusCounts();
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderget;
                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }


    }

}
