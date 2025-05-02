namespace Jammer.ProductModule.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string ProductURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; }
        public double Points { get; set; }
        public string Fullname { get; set; }
        public string Image { get; set; }   
    }
}
