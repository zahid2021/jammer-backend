using AutoMapper;
using Jammer.CartModule.DTOs;
using Jammer.DBContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Jammer.Utills;
using Jammer.UserModule.DTOs;
using Jammer.CartModule.Repositories.InterFace;
using Jammer.ResponseMessage;


namespace Jammer.CartModule.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly IMapper _mapper;
        readonly ICartRepositories _CartRepositories;

        public CartController(DapperContext context, IMapper mapper,ICartRepositories CartRepositories)
        {
            _context = context;
            _mapper = mapper;
            _CartRepositories = CartRepositories;   
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartRequestDTO request)
        {
            ResponseDTO response = new();
            

                var userId = Request.GetUser();

            try
            {
                if (await _CartRepositories.CreateCart(request,userId))
                {
                    response.Message=  MessageDisplay.cartadd;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.carterror;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
            
           
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            var userId = Request.GetUser();

            ResponseDTO response = new();
            try
            {
                var data =await _CartRepositories.GetUserCart(userId);
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.cartget;

                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartById(int cartId)
        {

            var userId = Request.GetUser();

            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                var data =await _CartRepositories.GetCartById(userId,cartId);

                response.Data = data;
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.cartget;

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

       
        [HttpPut]
        public async Task<IActionResult> UpdateCartItems(UpdateCartRequestDTO model)
        {
            ResponseDTO response = new();
            try
            {
                if (await _CartRepositories.UpdateCartItems(model))
                {
                    response.Message = MessageDisplay.cartupdate;

                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.cartupdateerror;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpDelete("{cartId}")]
        public async Task<IActionResult> DeleteCartItem(int cartId)
        {
            ResponseDTO response = new();
            try
            {
                if (await _CartRepositories.DeleteCartItem(cartId))
                {
                    response.Message = MessageDisplay.cartdelete;

                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.cartdeleteerror;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        } 
        [HttpDelete]
        public async Task<IActionResult> DeleteAllUserCart()
        {
            ResponseDTO response = new();
            try
            {
                var userId = Request.GetUser();

                if (await _CartRepositories.DeleteAllUserCart(userId))
                {
                    response.Message = MessageDisplay.cartdelete;

                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.cartdeleteerror;

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

    }

}
