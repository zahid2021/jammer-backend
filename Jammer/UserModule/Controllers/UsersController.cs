using AutoMapper;
using Jammer.DBContext;
using Jammer.ProductModule.DTOs;
using Jammer.ResponseMessage;
using Jammer.UserModule.DTOs;
using Jammer.UserModule.Models;
using Jammer.UserModule.Repositories.InterFace;
using Jammer.Utills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Jammer.UserModule.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly DapperContext _context;
        IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;
        private TokenHelper _tokenHelper;
        private readonly IMapper _mapper;
        public UsersController(DapperContext context, IWebHostEnvironment environment, TokenHelper tokenHelper,IMapper mapper, IUserRepository userRepository)
        {
            _context = context;
            _environment = environment;
            _tokenHelper = tokenHelper;
            _mapper = mapper;   
            _userRepository = userRepository;
        }
     


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignupAsync([FromForm]AddUserRequest request)
        {
            Console.WriteLine($"Received request: FullName: {request.FullName}, Email: {request.Email}");
                ResponseDTO response = new();
            try
            {

            GetUserResponse addUserRequest = _mapper.Map<AddUserRequest, GetUserResponse>(request);
            addUserRequest.PasswordHash = EncryptionDecryption.Encrypt(request.Password);
            addUserRequest.Image = await FileManage.UploadAsync(request.Image, _environment);

            //addUserRequest.RoleId = 1;
            int Id =await _userRepository.Signup(addUserRequest);
            if (Id != 0)
            {

                  var  token = _tokenHelper.GenerateToken(Id.ToString(), "Customer");

                    response.Data = token;
                    response.Message = "Customer registered successfully.";
                    return Ok(response);



                }
                else
                {
                    response.Message = MessageDisplay.error;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message == $"Duplicate entry '{request.Email}' for key 'email'" ? MessageDisplay.emailduplicated : MessageDisplay.error;
                return BadRequest(response);
            }

        } 

        [HttpPost]
        public async Task<IActionResult> SignupByAdminAsync([FromForm]AddUserRequestRole request)
        {
                ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {

            GetUserResponse addUserRequest = _mapper.Map<AddUserRequestRole, GetUserResponse>(request);
            addUserRequest.PasswordHash = EncryptionDecryption.Encrypt(request.Password);
            addUserRequest.Image = await FileManage.UploadAsync(request.Image, _environment);

            int Id =await _userRepository.Signup(addUserRequest);
            if (Id != 0)
            {
                string token="data";
                switch (addUserRequest.RoleId)
                {
                    case 1:
                        token = _tokenHelper.GenerateToken(Id.ToString(), "Admin");
                            response.Data=token;
                            response.Message = "Admin registered successfully.";
                        return Ok(response);
                        
                    case 2:
                        token = _tokenHelper.GenerateToken(Id.ToString(), "Customer");

                            response.Data = token;
                            response.Message = "Customer registered successfully.";
                            return Ok(response);
                       
                    case 3:
                        token = _tokenHelper.GenerateToken(Id.ToString(), "Manager");

                            response.Data = token;
                            response.Message = "Manager registered successfully.";
                            return Ok(response);


                        case 4:
                        token = _tokenHelper.GenerateToken(Id.ToString(), "DeliveryBoy");

                            response.Data = token;
                            response.Message = "Delivery Boy registered successfully.";
                            return Ok(response);


                        default:
                        token = "Invalid role specified.";
                        break;
                }
                response.Data = token;
               
                return Ok(response);

              
            }
                else
                {
                    response.Message = "Error Occurred While Processing Your Request";
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SignupAsync: {ex.Message}");
                    response.Message = ex.Message == $"Duplicate entry '{request.Email}' for key 'email'" ? MessageDisplay.emailduplicated : MessageDisplay.error;
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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
                ResponseDTO response = new();
            try
            {
                UserModel user = await _userRepository.LoginEmailPass(request);

                if (user == null || !EncryptionDecryption.Match(request.Password, user.PasswordHash))
                {
                    return BadRequest(MessageDisplay.LoginIncorrectDetailMessage);
                }
                else if (EncryptionDecryption.Match(request.Password, user.PasswordHash))
                {
                    string token = "";
                    switch (user.RoleId)
                    {
                        case 1:
                            token = _tokenHelper.GenerateToken(user.Id.ToString(), "Admin");
                            break;
                        case 2:
                            token = _tokenHelper.GenerateToken(user.Id.ToString(), "Customer");
                            break;
                        case 3:
                            token = _tokenHelper.GenerateToken(user.Id.ToString(), "Manager");
                            break;

                        case 4:
                            token = _tokenHelper.GenerateToken(user.Id.ToString(), "DeliveryBoy");
                            break;

                        default:
                            token = "Invalid role specified.";
                            break;
                    }
                    response.Message = MessageDisplay.LoginSuccessMessage;
                    response.Data = token;
                    return Ok(response);
                }
                response.Message = MessageDisplay.LoginIncorrectDetailMessage;
                return BadRequest(response);



            }catch(Exception ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] AddRoleRequest request)
        {
ResponseDTO response = new();
            try
            {
            var role = Request.GetRole();

            if (role == "Admin")
            {
                var addedRole =await _userRepository.AddRole(request);
                response.Data = addedRole;
                response.Message = "Role added successfully";
                return Ok(response);
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
            }catch(Exception ex)
            {
                response.Message= ex.Message == $"Duplicate entry '{request.Name}' for key 'UC_UserRole_Name'"? MessageDisplay.Roleduplicated : MessageDisplay.error;
                return BadRequest(response);

            }
            
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest request)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();

            if (role == "Admin")
            {
                var updatedRole = await _userRepository.UpdateRole(request);
                if (updatedRole)
                {
                    response.Message = "Role updated successfully";
                    return Ok(response);
                }
                else
                {
                    response.Message = "Role update failed";
                    return NotFound(response);
                }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();

            if (role == "Admin")
            {
                var deletedRole = await _userRepository.DeleteRole(id);
                if (deletedRole)
                {
                    response.Message = "Role deleted successfully";
                    return Ok(response);
                }
                else
                {
                    response.Message = "Role deletion failed";
                    return NotFound(response);
                }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {

 ResponseDTO response = new();
            try
            {
            var role = Request.GetRole();
            if (role == "Admin")
            {
                var users = await _userRepository.GetAllUser();
                response.Data = users;
                response.Message = "Get Successfully";
                return Ok(response);
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
            }catch(Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {

            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                var users = await _userRepository.GetAllRoles();
                response.Data=users;
                response.Message = "Get Successfully";
                return Ok(response);
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {

            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                var users = await _userRepository.GetRolesByID(id);
                response.Data=users;
                response.Message = "Get Successfully";
                return Ok(response);
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequest request)
        {
            var UserId = Request.GetUser();

            ResponseDTO response = new();
            GetUpdateRequest updateRequest = _mapper.Map<UpdateUserRequest, GetUpdateRequest>(request);
            updateRequest.Id = UserId;
            var existingUser = await _userRepository.GetUserById(UserId);
            if (existingUser == null || !EncryptionDecryption.Match(request.Password, existingUser.PasswordHash))
            {
                return BadRequest(MessageDisplay.LoginIncorrectDetailMessage);
            }
            else
            {
                updateRequest.Passwordhash = EncryptionDecryption.Encrypt(request.NewPassword);

            }
            if (existingUser != null)
            {
                if (request.Image != null)
                {
                    updateRequest.Image = await FileManage.UploadAsync(request.Image, _environment);
                }
                else
                {
                    updateRequest.Image = existingUser.Image; 
                }
                if (await _userRepository.UpdateUser(updateRequest))
                {
                    response.Message = "User updated successfully.";
                    return Ok(response);
                }
                else
                {
                    response.Message = "Error occurred while updating the user.";
                    return BadRequest(response);
                }
               
            }
            else
            {
                response.Message = "Error occurred while updating the user.";
                return BadRequest(response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserByAdmin([FromForm] UpdateUserByAdminRequest request)
        {

            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                GetUpdateByAdminRequest updateRequest = _mapper.Map<UpdateUserByAdminRequest, GetUpdateByAdminRequest>(request);
            updateRequest.Passwordhash = EncryptionDecryption.Encrypt(request.Password);
            var existingUser = await _userRepository.GetUserById(request.id.ToString()); 
            if (existingUser != null)
            {
                if (request.Image != null)
                {
                    updateRequest.Image = await FileManage.UploadAsync(request.Image, _environment);
                }
                else
                {
                    updateRequest.Image = existingUser.Image; 
                }
                if (await _userRepository.UpdateUserByAdmin(updateRequest))
                {
                    response.Message = "User updated successfully.";
                    return Ok(response);
                }
                else
                {
                    response.Message = "Error occurred while updating the user.";
                    return BadRequest(response);
                }
              
            }
            else
            {
                response.Message = "Error occurred while updating the user.";
                return BadRequest(response);
            }  }
                else
                {
                    response.Message = MessageDisplay.auth;
                    return Unauthorized(response);
                }
        }

        [HttpGet] 
        public async Task<IActionResult> GetUserById() 
        {
            string UserId = HttpContext.User.Claims.Single(o => o.Type == "UserId").Value;

            var user = await _userRepository.GetUserById(UserId);
            if (user == null)
            {
                return NotFound(new ResponseDTO { Message = "User not found" });
            }
            return Ok(user);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            string userId = HttpContext.User.Claims.Single(o => o.Type == "UserId").Value;
            ResponseDTO response = new();

            try
            {
                bool result = await _userRepository.DeleteUserById(userId);
                if (result)
                {
                    response.Message = "User account deleted successfully.";
                    return Ok(response);
                }
                else
                {
                    response.Message = "Error occurred while deleting the user account.";
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserByAdmin(string id)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();

            if (role == "Admin")
            {
                try
                {
                    bool result = await _userRepository.DeleteUserById(id);
                    if (result)
                    {
                        response.Message = "User deleted successfully.";
                        return Ok(response);
                    }
                    else
                    {
                        response.Message = "Error occurred while deleting the user.";
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
