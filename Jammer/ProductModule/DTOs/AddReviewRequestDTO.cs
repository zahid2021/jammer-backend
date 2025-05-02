namespace Jammer.ProductModule.DTOs
{
    public class AddReviewRequestDTO
    {

        public int ProductId { get; set; }
        public string Message { get; set; }
        public double Points { get; set; }
    }
}
