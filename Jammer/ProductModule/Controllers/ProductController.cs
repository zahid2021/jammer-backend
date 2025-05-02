using AutoMapper;
using Jammer.DBContext;
using Jammer.ProductModule.DTOs;
using Jammer.ProductModule.Models;
using Jammer.ProductModule.Repositories;
using Jammer.ResponseMessage;
using Jammer.UserModule.DTOs;
using Jammer.Utills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Jammer.ProductModule.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {

        readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public ProductController( IWebHostEnvironment environment, IMapper mapper,IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _environment = environment;
            _mapper = mapper;
        } 
        
        [HttpPost]
        public  async Task<IActionResult> AddProductWithImages([FromForm] AddUserProductRequest request)
        {
            

            Product addProductRequest = _mapper.Map<AddUserProductRequest,Product>(request);
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                if (await _productRepository.CreateProductAsync(addProductRequest,request,_environment)) {

                    response.Message = MessageDisplay.Productadd;
                    return Ok(response);
             }
             else
             {

                    response.Message = MessageDisplay.Producterror;
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
        [HttpPut]
        public async  Task<IActionResult> UpdateProductAsync([FromForm] UpdateProductRequestDTO request)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try { 

            Product product = _mapper.Map<UpdateProductRequestDTO,Product>(request);
            if ( await _productRepository.UpdateProductAsync(product, request, _environment) )
            {
                   response.Message= MessageDisplay.Productupdate;
                return Ok(response);
            }
            else
            {

                    response.Message = MessageDisplay.Productupdateerror;
                    return BadRequest(response);
            }
        } catch (Exception ex) {
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

        [HttpGet("{parentId}")]
        [AllowAnonymous]
        public async Task<IActionResult> FilterProductsByCategory(int parentId)
            {
            ResponseDTO response = new();
            try { 
             var data=await _productRepository.FilterProductsCategory(parentId, _mapper);

                response.Data = data;
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.Productget;

                return Ok(response);
            } catch (Exception ex) {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
           
            
        }
        [HttpGet]
        [AllowAnonymous]    
        public async Task<IActionResult> SearchProductsByName(string query)
        {
            ResponseDTO response = new();
            try
            {
                var data = await _productRepository.SearchProductsByName(query, _mapper);

                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.Productget;
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
        public async Task<IActionResult> GetProductsWithPaging(int pageNumber, int pageSize)
        {
            ResponseDTO response = new();
            try
            {
                var data = await _productRepository.GetProductsWithPaging(pageNumber,pageSize, _mapper);

                response.Data = data;
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.Productget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }

        }
       
        [HttpGet]
        public async Task<IActionResult> GetAllProductIdAndNames()
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                var data = await _productRepository.GetAllProductIdAndNames();

                response.Data = data;
                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.Productget;
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

                response.Message = MessageDisplay.Productupdateerror;
                return BadRequest(response);
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetProductById(int productId)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                var data = await _productRepository.GetProductById(productId, _mapper);

                response.Data = data;
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Productget;
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
} [HttpGet]
        public async Task<IActionResult> GetProductByIdBanner(int productId)
        {
            ResponseDTO response = new();
            //var role = Request.GetRole();
            //if (role == "Admin")
            //{
                try
            {
                var data = await _productRepository.GetProductByIdBanner(productId, _mapper);

                response.Data = data;
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Productget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
            //}
            //else
            //{
            //    response.Message = MessageDisplay.auth;
            //    return Unauthorized(response);
            //}
        }
        [HttpGet("url")]
        [AllowAnonymous]
        public async Task<IActionResult>  GetProductByURL(string url)
        {
            ResponseDTO response = new();
            try
            {
                var data = await _productRepository.GetProductByURL(url, _mapper);

                response.Data = data;
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Productget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProductStock(UpdateProductStockDTO request)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                
                    if(await _productRepository.UpdateProductStock(request))
                    {
                        response.Message = MessageDisplay.ProductUpdateSuccessfully;
                        return Ok(response);    
                    }
                    else{
                        response.Message= MessageDisplay.notFound ;
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                if (await _productRepository.DeleteProduct(id))
                {

                    response.Message = MessageDisplay.Productdelete;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Productdeleteerror;
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
