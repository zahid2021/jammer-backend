using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Jammer.ProductModule.DTOs
{
    public class UpdateProductRequestDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public string ProductURL { get; set; }
        public int CategoryId { get; set; }
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        public List<int> ImageIdsToDelete { get; set; } = new List<int>();
    }
}
