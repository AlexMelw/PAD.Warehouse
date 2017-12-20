using WarehouseAPI.WebApiHelpers;

namespace WarehouseAPI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using DTOs;
    using DTOs.Creational;
    using DTOs.Gettable;
    using DTOs.Patchable;
    using DTOs.Updatable;
    using HATEOAS;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Repositories.Context;
    using Repositories.Entities;

    //[Produces("application/json")]
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        private readonly EShopContext _context;
        private readonly ILogger<ProductsController> _logger;
        private EventId e;

        #region CONSTRUCTORS

        public ProductsController(EShopContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion

        //GET: api/Products
        [HttpGet]
        public IActionResult GetProducts([FromQuery] ProductFilter filter)
        {
            _logger.LogInformation($"Enter in {nameof(GetProduct)} method.");
            IQueryable<Product> products = _context.Products;

            if (filter.Label != null)
            {
                products = products?.Where(p => p.Label.Contains(filter.Label)) ?? Enumerable.Empty<Product>().AsQueryable();
            }

            if (filter.LPrice != 0)
            {
                products = products?.Where(p => p.Price <= filter.LPrice) ?? Enumerable.Empty<Product>().AsQueryable();
            }

            if (filter.GPrice != decimal.MaxValue)
            {
                products = products?.Where(p => p.Price >= filter.GPrice) ?? Enumerable.Empty<Product>().AsQueryable();
            }

            if (filter.Page.Size != int.MaxValue)
            {
                products = products?.Skip((filter.Page.Num - 1) * filter.Page.Size)
                    .Take(filter.Page.Size) ?? Enumerable.Empty<Product>().AsQueryable();
                
            }
            _logger.LogInformation("Successfully saved entity's modifications.");
            return TransformProductsToDTOs(products.ToList());
        }

        private IActionResult TransformProductsToDTOs(List<Product> products)
        {
            var hateoasProductDTOs = Mapper.Map<List<ProductToGetDTO>>(products);

            hateoasProductDTOs.ForEach(p =>
            {
                p.Links = new List<Link>
                {
                    new Link
                    {
                        Rel = $"/{nameof(Product)}",
                        Type = "GET",
                        Href = Url.Action("GetProduct", "Products", new { Id = p.Id }, Request.Scheme,
                            Request.Host.Host)
                    }
                };
            });

            return Ok(hateoasProductDTOs);
          
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] long id)
        {
            _logger.LogInformation($"Enter in {nameof(GetProduct)} method.");
            if (!ModelState.IsValid)
            {
                _logger.LogCritical(e, "Invalid model state.");
                return BadRequest(ModelState);
            }

            var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == id);

            var hateoasProductDTO = Mapper.Map<ProductToGetDTO>(product);

            if (product == null)
            {
                _logger.LogWarning("Product isn't provided or is invalid.");
                return NotFound();
            }

            hateoasProductDTO.Links = new List<Link>
            {
                new Link
                {
                    Rel = $"/{nameof(Product)}",
                    Type = "GET",
                    Href = Url.Action("GetProduct", "Products", new { Id = hateoasProductDTO.Id }, Request.Scheme,
                        Request.Host.Host)
                }
            };

            _logger.LogInformation("Successfully saved entity's modifications.");
            return Ok(hateoasProductDTO);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] long id, [FromBody] ProductToUpdateDTO productDTO)
        {
            _logger.LogInformation($"Enter in {nameof(PutProduct)} method.");
            if (!ModelState.IsValid)
            {
                _logger.LogCritical(e, "Invalid model state.");
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            Mapper.Map(productDTO, product);

            //product.Label = productDto.Label;
            //product.Price = productDto.Price;
            //product.Available = productDto.Available;
            //product.ImageUri = productDto.ImageUri;

            try
            {
                if (await _context.SaveChangesAsync() == 0)
                {
                    _logger.LogError("No entities were saved.");
                    return StatusCode(500, "A problem happened while handling your request.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    _logger.LogWarning($"Product with ID={id} not found");
                    return NotFound();
                }
                _logger.LogError("Modificarea simultana a entitatilor.");
                return StatusCode(500, $"Simultaneously attempt to modify {nameof(Product)} entity.");
            }
            _logger.LogInformation("Successfully saved entity's modifications.");
            return NoContent();
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] ProductToCreateDTO productDto)
        {
            _logger.LogInformation($"Enter in {nameof(PostProduct)} method.");
            if (!ModelState.IsValid)
            {
                _logger.LogCritical(e, "Invalid model state.");
                return BadRequest(ModelState);
            }

            //var product = new Product
            //{
            //    Label = productDto.Label,
            //    ImageUri = productDto.ImageUri,
            //    Available = productDto.Available,
            //    Price = productDto.Price
            //};

            Product product = Mapper.Map<Product>(productDto);

            _context.Products.Add(product);

            if (await _context.SaveChangesAsync() == 0)
            {
                _logger.LogError("No entities were saved.");
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] long id)
        {
            _logger.LogInformation($"Enter in {nameof(DeleteProduct)} method.");
            if (!ModelState.IsValid)
            {
                _logger.LogCritical(e, "Invalid model state.");
                return BadRequest(ModelState);
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                _logger.LogWarning("Product isn't provided or is invalid.");
                return NotFound();
            }

            _context.Products.Remove(product);

            if (await _context.SaveChangesAsync() == 0)
            {
                _logger.LogError("No entities were saved.");
                return StatusCode(500, "A problem happened while handling your request.");
            }
            _logger.LogInformation("Successfully update.");
            return StatusCode(204);
        }

        // PATCH: api/Products/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(long id, [FromBody] JsonPatchDocument<ProductToPatchDTO> patchDoc)
        {
            _logger.LogInformation($"Enter in {nameof(PatchProduct)} method.");
            if (patchDoc == null)
            {
                _logger.LogCritical("Product isn't provided or is invalid.");
                return BadRequest();
            }

            if (!ProductExists(id))
            {
                _logger.LogWarning($"Product with ID={id} not found");
                return NotFound();
            }

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            //var patchedProduct = new ProductToPatchDTO
            //{
            //    Id = product.Id,
            //    Label = product.Label,
            //    Price = product.Price,
            //    ImageUri = product.ImageUri,
            //    Available = product.Available
            //};

            var patchedProduct = Mapper.Map<ProductToPatchDTO>(product);

            patchDoc.ApplyTo(patchedProduct, ModelState);

            TryValidateModel(patchedProduct);

            if (!ModelState.IsValid)
            {
                _logger.LogCritical(e, "Invalid model state.");
                return BadRequest(ModelState);
            }

            if (patchedProduct.Id != product.Id)
            {
                _logger.LogCritical("An attempt of modifying entity's ID was occurred.");
                ModelState.AddModelError(nameof(product.Id), "Modification of ID isn't allowed.");
                return BadRequest(ModelState);
            }

            //product.Label = patchedProduct.Label;
            //product.ImageUri = patchedProduct.ImageUri;
            //product.Available = patchedProduct.Available;
            //product.Price = patchedProduct.Price;

            Mapper.Map(patchedProduct, product);

            if (_context.SaveChanges() == 0)
            {
                _logger.LogError("No entities were saved.");
                return StatusCode(500, "A problem happened while handling your request.");
            }
            _logger.LogInformation("Successfully saved entety's modifications.");
            return NoContent();
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}