namespace Jammer.CartModule.DTOs
{
    public class BannerDTO
    {
        public int Id { get; set; }

        public int LinkId { get; set; }
        public int Link { get; set; }
        public int CouponId { get; set; }
        public string Image { get; set; }

    } 
    public class BannerRequest
    {
        public int LinkId { get; set; }
        public int Link { get; set; }

        public int CouponId { get; set; }
        public IFormFile Image { get; set; }
    }
    public class BannerRequestDB
    {
        public  int LinkId { get; set; }

        public int CouponId { get; set; }
        public  int Link { get; set; }
        public  string Image { get; set; }
        public BannerRequestDB(int linkId, int link, string image,int couponid)
        {
            LinkId = linkId;
            Link = link;
            CouponId = couponid;
            Image = image;
        }
    }
}
