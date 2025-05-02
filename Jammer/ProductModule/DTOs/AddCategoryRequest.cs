
namespace Jammer.ProductModule.DTOs
{
    public class AddCategoryRequest
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public IFormFile ImagePath { get; set; }
    }


        public class CategoryRequest
        {
            public string Name { get; set; }
            public int? ParentId { get; set; }
            public string ImagePath { get; set; }


        }
}
