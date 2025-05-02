using System.Collections.Generic;

namespace Jammer.CartModule.DTOs
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Price { get; set; }
        public int CouponId { get; set; }
        public string ProductURL { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> ProductImages { get; set; } = new List<string>();
    }


    public class GetCartByIdDTO
    {
        public string UserId { get; set; }
        public int CartId { get; set; }
        public GetCartByIdDTO(string userid, int cartid)
        {
            UserId = userid;
            CartId = cartid;
        }
    }
    public class IntDTOs
    {
        public int id { get; set; }
        public IntDTOs(int data)
        {
            id = data;
        }
        public class CouponAndProductId
        {
            public int CouponId { get; set; }
            public int ProductId { get; set; }
            public CouponAndProductId(int couponid, int productid)
            {
                CouponId = couponid;
                ProductId = productid;
            }

        }
        public class ProductListDTOs
        {
            public List<int> id { get; set; }
            public ProductListDTOs(List<int>? data)
            {
                if (data == null)
                    id = [];
                else
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
}
