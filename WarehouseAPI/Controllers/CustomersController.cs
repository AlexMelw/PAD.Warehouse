namespace WarehouseAPI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using DTOs.Patchable;
    using HATEOAS;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Repositories.Context;
    using Repositories.Entities;

    //[Produces("application/json")]
    [Route("api/Customers")]
    public class CustomersController : Controller
    {
        private readonly EShopContext _context;

        #region CONSTRUCTORS

        public CustomersController(EShopContext context)
        {
            _context = context;
        }

        #endregion

        // GET: api/Customers
        [HttpGet]
        public IActionResult GetCustomers(bool withOrders = false, bool withOrderDetails = false)
        {
            if (withOrders && withOrderDetails)
            {
                return GetCustomersWithOrdersAndOrderDetails();
            }

            if (withOrders && !withOrderDetails)
            {
                return GetCustomersWithOrders();
            }

            return GetCustomersOnly();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer([FromRoute] long id,
            bool withOrders = false, bool withOrderDetails = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!CustomerExists(id))
            {
                return NotFound();
            }

            if (withOrders && withOrderDetails)
            {
                return await GetCustomerWithOrdersAndOrderDetailsAsync(id);
            }

            if (withOrders && !withOrderDetails)
            {
                return await GetCustomerWithOrdersAsync(id);
            }

            return await GetCustomerOnlyAsync(id);
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer([FromRoute] long id, [FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.SingleOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        // PATCH: api/Products/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCustomer(long id,
            [FromBody] JsonPatchDocument<CustomerToPatchDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!CustomerExists(id))
            {
                return NotFound();
            }

            Customer customer = await _context.Customers.FirstOrDefaultAsync(p => p.Id == id);

            var patchedCustomer = Mapper.Map<CustomerToPatchDTO>(customer);

            patchDoc.ApplyTo(patchedCustomer, ModelState);

            TryValidateModel(patchedCustomer);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (patchedCustomer.Id != customer.Id)
            {
                ModelState.AddModelError(nameof(customer.Id), "Modification of ID isn't allowed.");
                return BadRequest(ModelState);
            }

            Mapper.Map(patchedCustomer, customer);

            if (_context.SaveChanges() == 0)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        private IActionResult GetCustomersOnly()
        {
            List<Customer> customers = _context.Customers
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

        private IActionResult GetCustomersWithOrders()
        {
            List<Customer> customers = _context.Customers
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

        private IActionResult GetCustomersWithOrdersAndOrderDetails()
        {
            List<Customer> customers = _context.Customers
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

        private async Task<IActionResult> GetCustomerWithOrdersAndOrderDetailsAsync(long customerId)
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

        private async Task<IActionResult> GetCustomerWithOrdersAsync(long customerId)
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

        private async Task<IActionResult> GetCustomerOnlyAsync(long customerId)
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
}