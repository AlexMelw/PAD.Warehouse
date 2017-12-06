namespace WarehouseAPI.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Stubs;

    [Produces("application/json")]
    [Route("api/TeodorProducts")]
    public class TeodorProductsController : Controller
    {
        // GET: api/Products
        [HttpGet]
        public JsonResult Get()
        {
            var allProducts = ProductsDataSource.Default.Products;

            return new JsonResult(allProducts, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Products
        [HttpPost]
        public void Post([FromBody] string value) { }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) { }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id) { }
    }
}