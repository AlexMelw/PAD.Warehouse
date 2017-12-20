using Microsoft.EntityFrameworkCore.Extensions.Internal;
using WarehouseAPI.WebApiHelpers;

namespace WarehouseAPI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using DTOs.Gettable;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Repositories.Context;
    using Repositories.Entities;

    //[Produces("application/json")]
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        private readonly EShopContext _context;
        private readonly ILogger<OrderDetailsController> _logger;

        #region CONSTRUCTORS

        public OrdersController(EShopContext context,ILogger<OrderDetailsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion

        // GET: api/Orders
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderFilter filter)
        {
            _logger.LogInformation("Retrieve all the order.");
            IQueryable<Order> orders = _context.Orders;

            if (filter.FName != null)
            {
                orders = orders?.Where(o => o.Customer.FirstName == filter.FName) ?? Enumerable.Empty<Order>().AsQueryable();
            }

            if (filter.LName != null)
            {
                orders = orders?.Where(o => o.Customer.LastName == filter.LName) ?? Enumerable.Empty<Order>().AsQueryable();
            }

            if (filter.Address != null)
            {
                orders = orders?.Where(o => o.DeliveryAddress.Contains(filter.Address)) ?? Enumerable.Empty<Order>().AsQueryable();
            }

            if (filter.Page.Size != int.MaxValue)
            {
                orders = orders?.Skip((filter.Page.Num - 1) * filter.Page.Size)
                    .Take(filter.Page.Size) ?? Enumerable.Empty<Order>().AsQueryable();
            }

            List<Order> orderList = await orders.Include(o => o.OrderDetails)
                .ToListAsync();

            var orderDTOs = Mapper.Map<List<OrderToGetDTO>>(orderList);

            return Ok(orderDTOs);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] long id)
        {
            _logger.LogInformation($"Enter in {nameof(GetOrder)} method.");
            if (!ModelState.IsValid)
            {
                _logger.LogCritical("Invalid model state");
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                _logger.LogWarning("Order isn't provided or is invalid.");
                return NotFound();
            }

            var orderDTO = Mapper.Map<OrderToGetDTO>(order);
   
            return Ok(orderDTO);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder([FromRoute] long id, [FromBody] Order order)
        {
            _logger.LogInformation($"Enter in {nameof(PutOrder)} method.");
            if (!ModelState.IsValid)
            {
                _logger.LogCritical("Invalid model state");
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                _logger.LogCritical("");
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    _logger.LogWarning($"Order with ID ={ id} not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            _logger.LogInformation("Successfully saved entety's modifications.");
            return NoContent();
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            _logger.LogInformation($"Enter in {nameof(PostOrder)} method.");
            if (!ModelState.IsValid)
            {
                _logger.LogCritical("Invalid model state");
                return BadRequest(ModelState);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] long id)
        {
            _logger.LogInformation($"Enter in {nameof(DeleteOrder)} method.");
            if (!ModelState.IsValid)
            {
                _logger.LogCritical("Invalid model state");
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                _logger.LogWarning("Order isn't provided or is invalid.");
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully update.");
            return Ok(order);
        }

        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}