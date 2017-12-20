using WarehouseAPI.WebApiHelpers;

namespace WarehouseAPI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
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
    [Route("api/Customers")]
    public class CustomersController : Controller
    {
        private readonly EShopContext _context;
        private readonly ILogger<CustomersController> _logger;
        private readonly EventId e;

        #region CONSTRUCTORS

        public CustomersController(EShopContext context, ILogger<CustomersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion

        // GET: api/Customers
        [HttpGet]
        public IActionResult GetCustomers([FromQuery] CustomerFilter filter)
        {
            _logger.LogInformation("Retrieve all the customer.");
     
            IQueryable<Customer> customers = _context.Customers;

            if (filter.LName != null)
            {
                customers = customers?.Where(c => c.LastName == filter.LName) ?? Enumerable.Empty<Customer>().AsQueryable();
            }

            if (filter.FName != null)
            {
                customers = customers?.Where(c => c.FirstName == filter.FName) ?? Enumerable.Empty<Customer>().AsQueryable();
            }

            if (filter.FName_start_with != null)
            {
                customers = customers?.Where(c => c.FirstName.StartsWith(filter.FName_start_with)) ?? Enumerable.Empty<Customer>().AsQueryable();
            }

            if (filter.LName_start_with != null)
            {
                customers = customers?.Where(c => c.LastName.StartsWith(filter.LName_start_with)) ?? Enumerable.Empty<Customer>().AsQueryable();
            }

            if (filter.Page.Size != int.MaxValue)
            {
                customers = customers?.Skip((filter.Page.Num - 1) * filter.Page.Size)
                    .Take(filter.Page.Size) ?? Enumerable.Empty<Customer>().AsQueryable();

            }

            if (filter.WithOrders && filter.WithOrderDetails)
            {         
                return GetCustomersWithOrdersAndOrderDetails(customers);
            }

            if (filter.WithOrders && !filter.WithOrderDetails)
            {
                return GetCustomersWithOrders(customers);
            }

            return GetCustomersOnly(customers);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer([FromRoute] long id,
            bool withOrders = false, bool withOrderDetails = false)
        {
            _logger.LogInformation($"Enter in {nameof(GetCustomer)} method.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(e, "Invalid model state.");
                return BadRequest(ModelState);
            }

            if (!CustomerExists(id))
            {
                _logger.LogCritical(e, $"User with ID={id} is not found.");
                return NotFound();
            }

            if (withOrders && withOrderDetails)
            {
                _logger.LogInformation($"Request for both: orders and orders details of customer ID={id}.");
                return await GetCustomerWithOrdersAndOrderDetails(id);
            }

            if (withOrders && !withOrderDetails)
            {
                _logger.LogInformation($"Request for orders only of customer ID={id}.");
                return await GetCustomerWithOrders(id);
            }

            _logger.LogInformation($"Leaving the {nameof(GetCustomer)} method.");
            return await GetCustomerOnly(id);
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer([FromRoute] long id, [FromBody] CustomerToUpdateDTO customerDTO)
        {
            _logger.LogInformation($"Enter in {nameof(PutCustomer)} method.");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(e, "Invalid model state.");
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

            Mapper.Map(customerDTO, customer);

            try
            {
                _logger.LogInformation($"Salvam in BD customer cu ID={id}.");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    _logger.LogCritical(e, $"Concurrency violation while saving customer with ID={id}.");
                    return NotFound();
                }
                return StatusCode(500, $"Simultaneously attempt to modify {nameof(Customer)} entity.");
            }

            _logger.LogInformation($"Leaving the {nameof(PutCustomer)} method.");
            return NoContent();
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] CustomerToCreateDTO customerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = Mapper.Map<Customer>(customerDTO);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] long id)
        {
            _logger.LogInformation($"Enter in {nameof(DeleteCustomer)} method.");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(e, "Invalid model state.");
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.SingleOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                _logger.LogCritical(e, $"User with ID={id} is not found.");
                return NotFound();
            }
            _logger.LogInformation($"Remove customer with ID={id} and save changes.");
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Leaving the {nameof(DeleteCustomer)} method.");
            return Ok(customer);
        }

        // PATCH: api/Products/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCustomer(long id,
            [FromBody] JsonPatchDocument<CustomerToPatchDTO> patchDoc)
        {
            _logger.LogInformation($"Enter in {nameof(PatchCustomer)} method.");
            if (patchDoc == null)
            {
                _logger.LogWarning("Patch document isn't provided or is invalid.");
                return BadRequest();
            }

            if (!CustomerExists(id))
            {
                _logger.LogCritical(e, $"User with ID={id} is not found.");
                return NotFound();
            }

            Customer customer = await _context.Customers.FirstOrDefaultAsync(p => p.Id == id);

            var patchedCustomer = Mapper.Map<CustomerToPatchDTO>(customer);

            patchDoc.ApplyTo(patchedCustomer, ModelState);

            TryValidateModel(patchedCustomer);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(e, "Invalid model state.");
                return BadRequest(ModelState);
            }

            if (patchedCustomer.Id != customer.Id)
            {
                _logger.LogWarning("An attempt of modifying entity's ID was occurred.");
                ModelState.AddModelError(nameof(customer.Id), "Modification of ID isn't allowed.");
                return BadRequest(ModelState);
            }

            Mapper.Map(patchedCustomer, customer);

            if (_context.SaveChanges() == 0)
            {
                _logger.LogError("No entities were saved.");
                return StatusCode(500, "A problem happened while handling your request.");
            }
            _logger.LogInformation("Successfully saved entety's modifications.");
            return NoContent();
        }

        #region Helper Methods

        private IActionResult GetCustomersOnly(IQueryable<Customer> qCustomers)
        {
            List<Customer> customers = qCustomers
                .ToList();

            var hateoasCustomerDTOs = Mapper.Map<List<CustomerToGetDTO>>(customers);

            hateoasCustomerDTOs.ForEach(c =>
            {
                c.Links = new List<Link>
                {
                    new Link
                    {
                        Rel = $"/{nameof(Customer)}",
                        Type = "GET",
                        Href = Url.Action("GetCustomer", "Customers", new { Id = c.Id }, Request.Scheme,
                            Request.Host.Host)
                    }
                };
            });

            return Ok(hateoasCustomerDTOs);
        }

        private IActionResult GetCustomersWithOrders(IQueryable<Customer> qCustomers)
        {
            List<Customer> customers = qCustomers
                .Include(c => c.Orders)
                .ToList();

            var hateoasCustomerDTOs = Mapper.Map<List<CustomerToGetDTO>>(customers);

            hateoasCustomerDTOs.ForEach(c =>
            {
                c.Links = new List<Link>
                {
                    new Link
                    {
                        Rel = $"/{nameof(Customer)}",
                        Type = "GET",
                        Href = Url.Action("GetCustomer", "Customers", new { Id = c.Id }, Request.Scheme,
                            Request.Host.Host)
                    }
                };
                c.Orders.ForEach(o =>
                {
                    o.Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = $"/{nameof(Order)}",
                            Type = "GET",
                            Href = Url.Action("GetOrder", "Orders", new { Id = o.Id }, Request.Scheme,
                                Request.Host.Host)
                        },
                        new Link
                        {
                            Rel = $"/{nameof(Customer)}",
                            Type = "GET",
                            Href = Url.Action("GetCustomer", "Customers", new { Id = c.Id }, Request.Scheme,
                                Request.Host.Host)
                        }
                    };
                });
            });

            return Ok(hateoasCustomerDTOs);
        }

        private IActionResult GetCustomersWithOrdersAndOrderDetails(IQueryable<Customer> qCustomers)
        {
            List<Customer> customers = qCustomers
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderDetails)
                .ToList();

            var hateoasCustomerDTOs = Mapper.Map<List<CustomerToGetDTO>>(customers);

            hateoasCustomerDTOs.ForEach(c =>
            {
                c.Links = new List<Link>
                {
                    new Link
                    {
                        Rel = $"/{nameof(Customer)}",
                        Type = "GET",
                        Href = Url.Action("GetCustomer", "Customers", new { Id = c.Id }, Request.Scheme,
                            Request.Host.Host)
                    }
                };
                c.Orders.ForEach(o =>
                {
                    o.Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = $"/{nameof(Order)}",
                            Type = "GET",
                            Href = Url.Action("GetOrder", "Orders", new { Id = o.Id }, Request.Scheme,
                                Request.Host.Host)
                        },
                        new Link
                        {
                            Rel = $"/{nameof(Customer)}",
                            Type = "GET",
                            Href = Url.Action("GetCustomer", "Customers", new { Id = c.Id }, Request.Scheme,
                                Request.Host.Host)
                        }
                    };

                    o.OrderDetails.ForEach(od =>
                    {
                        od.Links = new List<Link>
                        {
                            new Link
                            {
                                Rel = $"/{nameof(OrderDetail)}",
                                Type = "GET",
                                Href = Url.Action("GetOrderDetail", "OrderDetails", new { Id = od.Id }, Request.Scheme,
                                    Request.Host.Host)
                            },
                            new Link
                            {
                                Rel = $"/{nameof(Order)}",
                                Type = "GET",
                                Href = Url.Action("GetOrder", "Orders", new { Id = o.Id }, Request.Scheme,
                                    Request.Host.Host)
                            },
                            new Link
                            {
                                Rel = $"/{nameof(Customer)}",
                                Type = "GET",
                                Href = Url.Action("GetCustomer", "Customers", new { Id = c.Id }, Request.Scheme,
                                    Request.Host.Host)
                            }
                        };
                    });
                });
            });

            return Ok(hateoasCustomerDTOs);
        }

        private async Task<IActionResult> GetCustomerWithOrdersAndOrderDetails(long customerId)
        {
            Customer customer = await _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderDetails)
                .SingleOrDefaultAsync(c => c.Id == customerId);

            var hateoasCustomerDTO = Mapper.Map<CustomerToGetDTO>(customer);

            hateoasCustomerDTO.Links = new List<Link>
            {
                new Link
                {
                    Rel = $"/{nameof(Customer)}",
                    Type = "GET",
                    Href = Url.Action("GetCustomer", "Customers", new { Id = hateoasCustomerDTO.Id }, Request.Scheme,
                        Request.Host.Host)
                }
            };
            hateoasCustomerDTO.Orders.ForEach(o =>
            {
                o.Links = new List<Link>
                {
                    new Link
                    {
                        Rel = $"/{nameof(Order)}",
                        Type = "GET",
                        Href = Url.Action("GetOrder", "Orders", new { Id = o.Id }, Request.Scheme,
                            Request.Host.Host)
                    },
                    new Link
                    {
                        Rel = $"/{nameof(Customer)}",
                        Type = "GET",
                        Href = Url.Action("GetCustomer", "Customers", new { Id = hateoasCustomerDTO.Id },
                            Request.Scheme,
                            Request.Host.Host)
                    }
                };

                o.OrderDetails.ForEach(od =>
                {
                    od.Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = $"/{nameof(OrderDetail)}",
                            Type = "GET",
                            Href = Url.Action("GetOrderDetail", "OrderDetails", new { Id = od.Id }, Request.Scheme,
                                Request.Host.Host)
                        },
                        new Link
                        {
                            Rel = $"/{nameof(Order)}",
                            Type = "GET",
                            Href = Url.Action("GetOrder", "Orders", new { Id = o.Id }, Request.Scheme,
                                Request.Host.Host)
                        },
                        new Link
                        {
                            Rel = $"/{nameof(Customer)}",
                            Type = "GET",
                            Href = Url.Action("GetCustomer", "Customers", new { Id = hateoasCustomerDTO.Id },
                                Request.Scheme,
                                Request.Host.Host)
                        }
                    };
                });
            });

            return Ok(hateoasCustomerDTO);
        }

        private async Task<IActionResult> GetCustomerWithOrders(long customerId)
        {
            Customer customer = await _context.Customers
                .Include(c => c.Orders)
                .SingleOrDefaultAsync(c => c.Id == customerId);

            var hateoasCustomerDTO = Mapper.Map<CustomerToGetDTO>(customer);

            hateoasCustomerDTO.Links = new List<Link>
            {
                new Link
                {
                    Rel = $"/{nameof(Customer)}",
                    Type = "GET",
                    Href = Url.Action("GetCustomer", "Customers", new { Id = hateoasCustomerDTO.Id }, Request.Scheme,
                        Request.Host.Host)
                }
            };
            hateoasCustomerDTO.Orders.ForEach(o =>
            {
                o.Links = new List<Link>
                {
                    new Link
                    {
                        Rel = $"/{nameof(Order)}",
                        Type = "GET",
                        Href = Url.Action("GetOrder", "Orders", new { Id = o.Id }, Request.Scheme,
                            Request.Host.Host)
                    },
                    new Link
                    {
                        Rel = $"/{nameof(Customer)}",
                        Type = "GET",
                        Href = Url.Action("GetCustomer", "Customers", new { Id = hateoasCustomerDTO.Id },
                            Request.Scheme,
                            Request.Host.Host)
                    }
                };
            });

            return Ok(hateoasCustomerDTO);
        }

        private async Task<IActionResult> GetCustomerOnly(long customerId)
        {
            Customer customer = await _context.Customers
                .SingleOrDefaultAsync(c => c.Id == customerId);

            var hateoasCustomerDTO = Mapper.Map<CustomerToGetDTO>(customer);

            hateoasCustomerDTO.Links = new List<Link>
            {
                new Link
                {
                    Rel = $"/{nameof(Customer)}",
                    Type = "GET",
                    Href = Url.Action("GetCustomer", "Customers", new { Id = hateoasCustomerDTO.Id }, Request.Scheme,
                        Request.Host.Host)
                }
            };
            return Ok(hateoasCustomerDTO);
        }

        private bool CustomerExists(long id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }

    #endregion
}