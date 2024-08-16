using MediatR;

namespace ProductService.API.Repo.Commands.Delete
{
    public class DeleteProductCommand : IRequest<int>
    {
        public int ProductId { get; set; }

        public DeleteProductCommand(int productId)
        {
            ProductId = productId;
        }
    }
}
