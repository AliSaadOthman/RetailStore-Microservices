using MediatR;
using ProductService.Models;

namespace ProductService.API.Repo.Commands.Create
{
    public class CreateCategoryCommand : IRequest<Guid>
    {
        public string Name { get; set; }

        public CreateCategoryCommand(string name)
        {
            Name = name;
        }
    }
}
