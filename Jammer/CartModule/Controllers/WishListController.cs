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
    public class WishListController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly IMapper _mapper;
        IWishListRepositories 
        _wishListRepositories;

        public WishListController(DapperContext context, IMapper mapper,
            IWishListRepositories wishListRepositories)
        {
            _context = context;
            _mapper = mapper;
            _wishListRepositories= wishListRepositories;
            
        }

        [HttpPost]
        public async Task<IActionResult> AddWishList([FromBody] AddToCartRequestDTO request)
        {

            var userId = Request.GetUser();

            ResponseDTO response = new();
            try
            {
                if (await _wishListRepositories.CreateWishList(request, userId))
                {
                    response.Message = MessageDisplay.Wishlistadd;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Wishlisterror;
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
        public async Task<IActionResult> GetUserWishList()
        {
            var userId = Request.GetUser();

            ResponseDTO response = new();
            try
            {
                var data =await _wishListRepositories.GetUserWishList(userId);
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.Wishlistget;
                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWishListById(int id)
        {

            var userId = Request.GetUser();

            ResponseDTO response = new();
            try
            {
                var data =await _wishListRepositories.GetWishListById(userId, id);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Wishlistget;
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
        public async Task<IActionResult> UpdateWishListItem(UpdateCartRequestDTO model)
        {
            ResponseDTO response = new();
            try
            {
                if (await _wishListRepositories.UpdateWishLists(model))
                {
                    response.Message = MessageDisplay.Wishlistupdate;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Wishlistupdateerror;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.Wishlisterror;
                return BadRequest(response);
            }
        }  [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWishListItem(int id)
        {
            ResponseDTO response = new();
            try
            {
                if (await _wishListRepositories.DeleteWishListItem(id))
                {
                    response.Message = MessageDisplay.Wishlistdelete;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Wishlistdeleteerror;
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
        public async Task<IActionResult> DeleteAllUserWishList()
        {
            ResponseDTO response = new();
            try
            {
                var userId = Request.GetUser();

                if (await _wishListRepositories.DeleteAllUserWishList(userId))
                {
                    response.Message = MessageDisplay.Wishlistdelete;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Wishlistdeleteerror;
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
