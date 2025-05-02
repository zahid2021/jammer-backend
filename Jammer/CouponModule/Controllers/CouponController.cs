using AutoMapper;
using Jammer.CouponModule.DTOs;
using Jammer.CouponModule.Models;
using Jammer.DBContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Jammer.UserModule.DTOs;
using Jammer.CouponModule.Repositories.InterFace;
using Jammer.OrderModule.DTOs;
using Jammer.ResponseMessage;
using Jammer.Utills;
using System.Threading.Tasks;

namespace Jammer.CouponModule.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly IMapper _mapper;
        ICouponRepositories _couponRepositories;

        public CouponController(DapperContext context, IMapper mapper,ICouponRepositories couponRepositories)
        {
            _context = context;
            _mapper = mapper;
            _couponRepositories = couponRepositories;
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CouponDTO request)
        {
                ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
            try
            {
                if (await _couponRepositories.CreateCoupon(request))
                {
                    response.Message = MessageDisplay.couponadd;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.couponerror;
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
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCoupons()
        {
            ResponseDTO response = new();
            try
            {
                var data =await _couponRepositories.GetAllCoupons();
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.couponget;
                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCoupon( CouponDTO request)
        {

            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                if (await _couponRepositories.UpdateCoupon(request))
                {
                    response.Message = MessageDisplay.couponupdate;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.couponupdateerror;
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


        [HttpPost]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequestDTO request)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                var data =await _couponRepositories.ApplyCoupon(request);

                response.Data = data;
                response.Message = data == false ? MessageDisplay.notFound : MessageDisplay.couponapply;

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


      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            ResponseDTO response = new();
            try
            {
                var data =await _couponRepositories.GetCouponById(id);

                response.Data = data;
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.couponget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetRandomCouponProducts()
        {
            ResponseDTO response = new();
            try
            {
                var data = await _couponRepositories.GetRandomCouponProductsAsync();
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.couponproductget;
                response.Data = data;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response); 
            }
        } 
        [HttpGet("{couponId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCoupon(string couponId)

        {
            ResponseDTO response = new();
            try
            {
                var data = await _couponRepositories.GetProductsByCouponAsync(couponId);
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.couponproductget;
                response.Data = data;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response); 
            }
        }
         [HttpGet("{couponId}/{productId}")]
        [AllowAnonymous]

        public async Task<IActionResult> GetProductByProductIdCouponIdAsync(string couponId, string productId)

        {
            ResponseDTO response = new();
            try
            {
                var data = await _couponRepositories.GetProductByProductIdCouponIdAsync(couponId,productId);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.couponproductget;
                response.Data = data;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response); 
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByDiscountPercentage(int discountPercentage)
        {
            ResponseDTO response = new();

            try
            {
                var data = await _couponRepositories.GetProductsByDiscountPercentage(discountPercentage);

                response.Data = data;
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.couponproductget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }  
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCouponsWithProductsAsync()
        {
            ResponseDTO response = new();
            try
            {
                var data = await _couponRepositories.GetCouponsWithProductsAsync();

                response.Data = data;
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.couponproductget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByDiscountRange(decimal minDiscount, decimal maxDiscount)
        {
            ResponseDTO response = new();
            try
            {
                var data = await _couponRepositories.GetProductsByDiscountRange(minDiscount, maxDiscount);

                response.Data = data;
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.couponproductget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        } 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
                ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
            try
            {
                if (await _couponRepositories.DeleteCoupon(id))
                {
                    response.Message = MessageDisplay.coupondelete;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.coupondeleteerror;
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

    }



}
