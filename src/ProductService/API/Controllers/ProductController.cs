using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductService.Events;
using System.Diagnostics;
using ProductService.Models;
using ProductService.Infrastructure.EventBus;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using ProductService.API.Services;
using ProductService.API.Repo.Commands.Create;
using Microsoft.AspNetCore.Authorization;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IEventBus _eventBus;
        private readonly IProductServiceAPI _service;

        public ProductController(ILogger<ProductController> logger, IEventBus eventBus, IProductServiceAPI service)
        {
            _logger = logger;
            _eventBus = eventBus;
            _service = service;
        }

        [HttpPost]
        [Route("CreateProduct")]
        [Authorize(Policy = "create:products")]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            var productId = await _service.CreateProduct(command);

/*            var productCreatedEvent = new ProductDeletedEvent(productId);
            _eventBus.Publish(productDeletedEvent);*/

            return Ok(new { productId });
        }

        [HttpPost]
        [Route("CreateCategory")]
        [Authorize(Policy = "create:categories")]
        public async Task<IActionResult> CreateCategory(CreateCategoryCommand command)
        {
            var caregoryId = await _service.CreateCategory(command);

            return Ok(new { caregoryId });
        }
    }
}
