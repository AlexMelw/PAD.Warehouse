namespace WarehouseAPI.DTOs.Gettable
{
    using System.Collections.Generic;
    using HATEOAS;

    public class ProductToGetDTO
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public List<Link> Links { get; set; }
        public string ImageUri { get; set; }
    }
}