using Jammer.ProductModule.DTOs;
using Jammer.ProductModule.Models;

namespace Jammer.ProductModule.Repositories.InterFace
{
    public interface IReviewRepository
    {
        public Task<bool> AddReviewAsync(AddReviewRequestDTO request,string userId);
        public Task<bool> DeleteReview(int id);
        public Task<List<Review>> GetReviews(int productId);
    }
}
