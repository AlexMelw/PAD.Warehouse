namespace WarehouseAPI.DTOs.Gettable
{
    using System;
    using System.Collections.Generic;
    using HATEOAS;

    public class OrderToGetDTO
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }

        public DateTime DateCreated { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal Total { get; set; }

        public List<OrderDetailToGetDTO> OrderDetails { get; set; }
            = new List<OrderDetailToGetDTO>();

        public List<Link> Links { get; set; }
    }
}