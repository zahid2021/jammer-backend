using Microsoft.AspNetCore.Mvc;
using Jammer.CartModule.Repositories.InterFace;
using Jammer.Utills;
using Jammer.UserModule.DTOs;
using Jammer.CartModule.DTOs;
using Jammer.ResponseMessage;

namespace Jammer.CartModule.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BannerController : ControllerBase
    {
        private readonly IBannerRepository _bannerRepository;

        private readonly IWebHostEnvironment _environment;

        public BannerController(IBannerRepository bannerRepository, IWebHostEnvironment
            webHostEnvironment)
        {
            _bannerRepository = bannerRepository;
            _environment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> AddBanner([FromForm] BannerRequest request)
        {
            ResponseDTO response = new();
                var role = Request.GetRole();
            if (role == "Admin")
            {
            try
            {
                string imagePath = await FileManage.UploadAsync(request.Image, _environment);

                    
                int bannerId = await _bannerRepository.CreateBannerAsync(new BannerRequestDB(request.LinkId, request.Link, imagePath,request.CouponId));

                    if (bannerId > 0)
                    { response.Message = MessageDisplay.BannerAdd;
                        response.Data = bannerId;
                    return Ok(response);
                    }
                   

                

                response.Message = MessageDisplay.Banneradderror;
                return BadRequest(response);
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
        public async Task<IActionResult> GetAllBanners()
        {  ResponseDTO response = new();
           
            try
            {
                var banners = await _bannerRepository.GetAllBannersAsync();
                response.Data = banners;
               response.Message = banners==null? MessageDisplay.notFound: MessageDisplay.Bannerget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
           
         
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteBanner(int Id)
        {
            ResponseDTO response = new();
            try
            {
                if (await _bannerRepository.DeleteBanner(Id))
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
