using AutoMapper;
using Jammer.DBContext;
using Jammer.ProductModule.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using Jammer.ResponseMessage;
using Jammer.UserModule.DTOs;
using Jammer.ProductModule.Repositories.InterFace;
using Jammer.Utills;
using Microsoft.AspNetCore.Authorization;

namespace Jammer.ProductModule.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
     
            private readonly DapperContext _context;
            private readonly IMapper _mapper;

        private readonly IWebHostEnvironment _environment;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(DapperContext context, IMapper mapper, ICategoryRepository categoryRepository, IWebHostEnvironment
            webHostEnvironment)
        {
            _categoryRepository = categoryRepository;
            _context = context;
            _mapper = mapper;
            _environment = webHostEnvironment;
        }            
           
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromForm] AddCategoryRequest request)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
            {
                if (request.ImagePath != null)
                {
                    string imagePath = await FileManage.UploadAsync(request.ImagePath, _environment);

                    var addCategoryRequest = new CategoryRequest { Name = request.Name, ParentId = request.ParentId, ImagePath = imagePath};

                    if (await _categoryRepository.AddCategory(addCategoryRequest))

                    {

                        response.Message = MessageDisplay.categoryadd;
                        return Ok(response);
                    }
                    else
                    {

                        response.Message = MessageDisplay.categoryerror;
                        return BadRequest(response);
                    }
                }
                else
                {
                    response.Message = "Image file is required.";
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message=="Cannot add or update a child row: a foreign key constraint fails (`jammer`.`categories`, CONSTRAINT `categories_ibfk_1` FOREIGN KEY (`ParentId`) REFERENCES `categories` (`Id`) ON DELETE CASCADE)"?MessageDisplay.categoryParenterror:
                    MessageDisplay.error;
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
        public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategory request)
        {
            var role = Request.GetRole();
            ResponseDTO response = new();

            if (role == "Admin")
            {
                try
                {
                    var existingCategory = await _categoryRepository.GetCategoryById(request.Id);

                    var categoryToUpdate = _mapper.Map<UpdateCategory, UpdateCategoryDTO>(request);
                    if (existingCategory != null)
                    {
                        if (string.IsNullOrEmpty(request.Name))
                        {
                            request.Name = existingCategory.Name;
                        }

                        if (request.ImagePath != null)
                        {
                            categoryToUpdate.ImagePath = await FileManage.UploadAsync(request.ImagePath, _environment);
                        }
                        else
                        {
                            categoryToUpdate.ImagePath = existingCategory.ImagePath;
                        }


                        if (await _categoryRepository.UpdateCategory(categoryToUpdate))
                        {
                            response.Message = MessageDisplay.categoryupdate;
                            return Ok(response);
                        }
                        else
                        {
                            response.Message = MessageDisplay.categoryupdateerror;
                            return BadRequest(response);
                        }
                    }
                    else
                    {
                        response.Message = "Category not found.";
                        return NotFound(response);
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message == "Cannot add or update a child row: a foreign key constraint fails (`jammer`.`categories`, CONSTRAINT `categories_ibfk_1` FOREIGN KEY (`ParentId`) REFERENCES `categories` (`Id`) ON DELETE CASCADE)" ? MessageDisplay.categoryParenterror :
                   MessageDisplay.error;
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
        public ActionResult GetAllCategories()
        {
            ResponseDTO response = new();
            try
            {
                var data =  _categoryRepository.GetAllCategories();

                response.Data = data;

                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.categoryget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCategoryById(int id)
        {
            ResponseDTO response = new();
            try
            {
                var category = await _categoryRepository.GetCategoryById(id); 

                if (category == null)
                {
                    response.Message = MessageDisplay.notFound;
                    return NotFound(response);
                }

                response.Data = category;
                response.Message = MessageDisplay.categoryget;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteCategory(int id)
            {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                using IDbConnection db = _context.CreateConnection();

                string query = "DELETE FROM Categories WHERE Id = @Id;";

                try
                {
                    int rowsAffected = await db.ExecuteAsync(query, new { Id = id });
                if (rowsAffected == 0) {    response.Message = MessageDisplay.categorydeleteerror;
                return NotFound(response);
}


                response.Message = MessageDisplay.categoryget;
                return Ok(response);
                }
                catch (Exception)
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
