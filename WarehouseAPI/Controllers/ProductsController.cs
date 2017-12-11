namespace WarehouseAPI.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using DTOs;
    using DTOs.Creational;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Repositories.Context;
    using Repositories.Entities;

    //[Produces("application/json")]
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        private readonly EShopContext _context;

        #region CONSTRUCTORS

        public ProductsController(EShopContext context)
        {
            _context = context;
        }

        #endregion

        //GET: api/Products
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(_context.Products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] long id, [FromBody] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productDto.Id)
            {
                return BadRequest();
            }

            //_context.Entry(product).State = EntityState.Modified;

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productDto.Id);

            product.Label = productDto.Label;
            product.Price = productDto.Price;
            product.Available = productDto.Available;
            product.ImageUri = productDto.ImageUri;

            try
            {
                if (await _context.SaveChangesAsync() == 0)
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                return StatusCode(500, $"Simultaneously attempt to modify {nameof(Product)} entity.");
            }

            return NoContent();
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] ProductToCreateDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Label = productDto.Label,
                ImageUri = productDto.ImageUri,
                Available = productDto.Available,
                Price = productDto.Price
            };

            _context.Products.Add(product);

            if (await _context.SaveChangesAsync() == 0)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            if (await _context.SaveChangesAsync() == 0)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return StatusCode(204);
        }

        // PATCH: api/Products/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(long id, [FromBody] JsonPatchDocument<ProductToPatchDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!ProductExists(id))
            {
                return NotFound();
            }

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            var patchedProduct = new ProductToPatchDTO
            {
                Id = product.Id,
                Label = product.Label,
                Price = product.Price,
                ImageUri = product.ImageUri,
                Available = product.Available
            };

            patchDoc.ApplyTo(patchedProduct, ModelState);

            TryValidateModel(patchedProduct);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (patchedProduct.Id != product.Id)
            {
                ModelState.AddModelError(nameof(product.Id), "Modification of ID isn't allowed.");
                return BadRequest(ModelState);
            }

            product.Label = patchedProduct.Label;
            product.ImageUri = patchedProduct.ImageUri;
            product.Available = patchedProduct.Available;
            product.Price = patchedProduct.Price;

            if (_context.SaveChanges() == 0)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}