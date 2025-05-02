using Jammer.CouponModule.DTOs;
using Jammer.CouponModule.Models;
using Jammer.OrderModule.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Jammer.CouponModule.Repositories.InterFace
{
    public interface ICouponRepositories
    {
        Task<bool> CreateCoupon(CouponDTO couponDTO);
        Task<List<CouponDTO>> GetAllCoupons();
        Task<bool> UpdateCoupon(CouponDTO couponDTO);
        Task<bool> DeleteCoupon(int id);
        Task<bool> ApplyCoupon( ApplyCouponRequestDTO request);
        Task<List<RandomCouponProductDTO>> GetRandomCouponProductsAsync();
        Task<List<RandomCouponProductDTO>> GetProductsByCouponAsync(string couponId);
        Task<List<CouponWithProductsDTO>> GetCouponsWithProductsAsync();
        Task<RandomCouponProductDTO?> GetProductByProductIdCouponIdAsync(string couponId, string productId);

        Task<CouponDTO?> GetCouponById(int id);
        Task<List<RandomCouponProductDTO>> GetProductsByDiscountPercentage(int discountPercentage);
        Task<List<RandomCouponProductDTO>> GetProductsByDiscountRange(decimal minDiscount, decimal maxDiscount);
    }
}
