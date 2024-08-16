namespace ProductService.Events
{
    public class ProductDeletedEvent
    {
        public string ProductId { get; set; }

        public ProductDeletedEvent(string productId)
        {
            ProductId = productId;
        }
    }
}
