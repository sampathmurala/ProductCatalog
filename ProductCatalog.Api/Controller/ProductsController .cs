using Microsoft.AspNetCore.Mvc;
using ProductCatalog.DataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCatalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }

        // GET: api/products
        /// <summary>
        /// Retrieves all products from the catalog.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _repository.GetAllAsync();
            return Ok(products);
        }

        // GET: api/products/{id}
        /// <summary>
        /// Retrieves a single product by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: api/products
        /// <summary>
        /// Creates a new product in the catalog.
        /// </summary>
        /// <param name="product">The product object to create.</param>
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            var newId = await _repository.CreateAsync(product);
            product.Id = newId;
            return CreatedAtAction(nameof(GetById), new { id = newId }, product);
        }

        // PUT: api/products/{id}
        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="product">The updated product object.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var success = await _repository.UpdateAsync(product);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/products/{id}
        /// <summary>
        /// Deletes a product by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
