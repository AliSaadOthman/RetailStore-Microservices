using MediatR;
using ProductService.Models;

namespace ProductService.API.Repo.Commands.Update
{
    public class UpdateProductCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public UpdateProductCommand(string name, string description, decimal price, int categoryId, Category category)
        {
            Name = name;
            Description = description;
            Price = price;
            CategoryId = categoryId;
            Category = category;
        }
    }
}
