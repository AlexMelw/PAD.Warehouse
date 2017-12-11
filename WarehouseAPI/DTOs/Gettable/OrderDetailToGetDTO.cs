namespace WarehouseAPI.Controllers
{
    using System.Collections.Generic;
    using HATEOAS;

    public class OrderDetailToGetDTO
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long OrderId { get; set; }

        public int Quantity { get; set; }

        public List<Link> Links { get; set; }

    }
}