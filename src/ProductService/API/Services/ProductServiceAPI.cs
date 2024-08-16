using MediatR;
using ProductService.API.Repo.Commands.Create;

namespace ProductService.API.Services
{
    public class ProductServiceAPI : IProductServiceAPI
    {
        private readonly IMediator _mediator;

        public ProductServiceAPI(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Guid> CreateProduct(CreateProductCommand command)
        {
            var productId = await _mediator.Send(command);
            return productId;
        }

        public async Task<Guid> CreateCategory(CreateCategoryCommand command)
        {
            var categoryId = await _mediator.Send(command);
            return categoryId;
        }
    }
}
