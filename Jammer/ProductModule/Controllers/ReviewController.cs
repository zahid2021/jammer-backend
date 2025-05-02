using AutoMapper;
using Jammer.DBContext;
using Jammer.ProductModule.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Jammer.UserModule.DTOs;
using Jammer.ProductModule.Repositories.InterFace;
using Jammer.Utills;
using Jammer.ResponseMessage;

namespace Jammer.ProductModule.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly DapperContext _context;
        readonly IReviewRepository _reviewRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public ReviewController(DapperContext context, IWebHostEnvironment environment, IMapper mapper,IReviewRepository reviewRepository)
        {   _reviewRepository= reviewRepository;
            _context = context;
            _environment = environment;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequestDTO request)
        {

            var userId = Request.GetUser();
            ResponseDTO response = new();
            try
            {
                if (await _reviewRepository.AddReviewAsync(request, userId))
                {

                    response.Message = MessageDisplay.Reviewadd;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Reviewerror;
                    return BadRequest(response);
                }
            }
            catch (Exception ex) {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
            }
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetReviews(int productId)
        {
            ResponseDTO response = new();
            try
            {
                var data =await _reviewRepository.GetReviews(productId);

                response.Data = data;

                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.Reviewget;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpDelete("deleteReview/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {

            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
                {
                    if (await _reviewRepository.DeleteReview(id))
                    {
                        response.Message = MessageDisplay.Reviewdelete;
                        return Ok(response);
                    }
                    else
                    {

                        response.Message = MessageDisplay.Reviewdeleteerror;
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
