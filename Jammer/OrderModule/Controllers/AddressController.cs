using AutoMapper;
using Dapper;
using Jammer.DBContext;
using Jammer.OrderModule.Repositories.InterFace;
using Jammer.ResponseMessage;
using Jammer.UserModule.DTOs;
using Jammer.Utills;
using Microsoft.AspNetCore.Mvc;
using Jammer.OrderModule.DTOs;
using Microsoft.AspNetCore.Authorization;
using Jammer.OrderModule.Models;

namespace Jammer.OrderModule.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly DapperContext _context;
        readonly IAddressRepositories _addressRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public AddressController(DapperContext context, IWebHostEnvironment environment, IMapper mapper, IAddressRepositories addressRepositories)
        {
            _addressRepository = addressRepositories;
            _context = context;
            _environment = environment;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressRequestDTO request)
        {

            var userId = Request.GetUser();
            ResponseDTO response = new();
            try
            {
                AddAddressRequest model=new AddAddressRequest( request,userId);
                if (await _addressRepository.AddAddress(model))
                {

                    response.Message = MessageDisplay.Addressadd;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Addresserror;
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
        public async Task<IActionResult> GetAddresss()
        {
            ResponseDTO response = new();
            try
            {

                var userId = Request.GetUser();
                var data = await _addressRepository.GetAddress(userId);

                response.Data = data;

                response.Message = data == null|| data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.Addressget;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAddress([FromBody] Address request)
        {
            ResponseDTO response = new();
            try
            {
                if (await _addressRepository.UpdateAddress(request))
                {
                    response.Message = MessageDisplay.AddressUpdated;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.AddressUpdateError;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {

            ResponseDTO response = new();
            try
            {
                if (await _addressRepository.DeleteAddress(id))
                {
                    response.Message = MessageDisplay.Addressdelete;
                    return Ok(response);
                }
                else
                {

                    response.Message = MessageDisplay.Addressdeleteerror;
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