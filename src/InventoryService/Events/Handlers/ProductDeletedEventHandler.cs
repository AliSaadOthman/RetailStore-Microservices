using ProductService.Events.Handlers;
using ProductService.Events;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryService.Events.Handlers
{
    public class ProductDeletedEventHandler : IEventHandler<ProductDeletedEvent>
    {
        public async Task Handle(ProductDeletedEvent @event)
        {
            Console.WriteLine("Handler Reached");
        }
    }
}
