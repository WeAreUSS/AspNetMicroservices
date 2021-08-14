using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;

namespace Catalog.API.Controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;


        // Because we are injecting the IProductRepository object into the create method,
        // and, because we have the ICatalogContext is being referenced in the ProductRepository,
        // we must include them in the Start.cs/ConfigureServices method file as injected objects.
        //       services.AddScoped<ICatalogContext, CatalogContext>();
        //       services.AddScoped<IProductRepository, ProductRepository>();
        //       (AddScoped is used because they are part of HTTP activity)
        //==================================================================
        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        #region Get Operations

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        // NOTE: The template below is used to pass in the id
        //       This is used to distinguish from the base GET "GetProducts" method defined above
        // ALSO: Notice how we enumerated the two return types:
        //       ---------------------------------------------
        //    [ProducesResponseType((int)HttpStatusCode.NotFound)]
        //    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        //=====================================================

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _repository.GetProduct(id);
            if (product == null)
            {
                _logger.LogError($"Product with id: {id}, not found.");
                return NotFound();
            }
            return Ok(product);
        }

        // NOTE: The Route is decorated with action(which is GetProductByCategory)/category as category is the parameter passed into the method.
        //       This is used to distinguish from the base GET "GetProducts" method defined above
        //=====================================================

        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var products = await _repository.GetProductByCategory(category);
            return Ok(products);
        }

        #endregion Get Operations


        #region CRUD Operations

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _repository.CreateProduct(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        // NOTE: We are using IActionResult in the following two methods as there is no return value.
        //       Also, note we are only returning the 200 response value "OK"
        //=====================================================

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _repository.UpdateProduct(product));
        }


        // Again, the template below is used to pass in the id
        // ===================================================

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _repository.DeleteProduct(id));
        }

        #endregion CRUD Operations
    }

}
