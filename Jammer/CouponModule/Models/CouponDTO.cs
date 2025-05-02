using Jammer.OrderModule.DTOs;

namespace Jammer.CouponModule.Models
{
    public class CouponDTO
       
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public string DiscountType { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
   
    public class CouponWithProductsDTO
    {
        public CouponDTO Coupon { get; set; }
        public List<RandomCouponProductDTO> Products { get; set; }
    }
    public class IntDTOs
    {
        public int id { get; set; }
        public IntDTOs(int data)
        {
            id = data;
        }

    }
  
    public class StringDTOs
    {
        public String id { get; set; }
        public StringDTOs(String data)
        {
            id = data;
        }
       
    }
}
