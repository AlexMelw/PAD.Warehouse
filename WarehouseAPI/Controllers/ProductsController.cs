namespace WarehouseAPI.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using DTOs;
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
            }

            return NoContent();
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] ProductDTO productDto)
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

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}