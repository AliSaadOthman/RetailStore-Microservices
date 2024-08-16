using ProductService.API.Repo.Commands.Create;

namespace ProductService.API.Services
{
    public interface IProductServiceAPI
    {
        public Task<Guid> CreateProduct(CreateProductCommand command);
        public Task<Guid> CreateCategory(CreateCategoryCommand command);
    }
}
