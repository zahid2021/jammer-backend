using Jammer.CartModule.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Jammer.CartModule.Repositories.InterFace
{
    public interface IBannerRepository
    {
        Task<int> CreateBannerAsync(BannerRequestDB imagePath);
        Task<IEnumerable<BannerDTO>> GetAllBannersAsync();
         Task<bool> DeleteBanner(int Id);
    }
}
