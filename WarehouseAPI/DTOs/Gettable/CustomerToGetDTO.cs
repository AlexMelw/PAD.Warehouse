namespace WarehouseAPI.Controllers
{
    using System.Collections.Generic;
    using HATEOAS;

    public class CustomerToGetDTO
    {
        public long Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<OrderToGetDTO> Orders { get; set; }
            = new List<OrderToGetDTO>();

        public List<Link> Links { get; set; }

    }
}